using System;
using System.Collections.Generic;
using System.IO;
using SauceControl.Blake2Fast;

namespace NodeAPIClient.Models
{

    public class Block
    {
        public byte Version { get; set; }

        public Primitives.Hash PreviousHash { get; set; }

        public Primitives.Hash Hash { get; set; }

        public UInt64 Sequence { get; set; }

        public List<UserField> UserFields { get; set; }

        public Money RoundCost { get; set; }

        public List<Transaction> Transactions { get; set; }

        public List<WalletIntroduce> IntroducedWallets { get; set; }

        public List<ConsensusMember> TrustedNodes { get; set; }

        // trusted index in table -> signature
        public List<KeyValuePair<int, Primitives.Signature>> TrustedApproval { get; set; }

        public List<ContractConfirmation> ContractsApproval { get; set; }

        internal static ResponseBlock Parse(byte[] bytes)
        {
            try
            {
                if (bytes == null)
                {
                    return new ResponseBlock() { Success = false, Message = "Input bytes array is null" };
                }

                using (BinaryReader bin = new BinaryReader(new MemoryStream(bytes)))
                {
                    int hashing_len = 0;
                    Block block = new Block();
                    block.Version = bin.ReadByte();
                    int len = bin.ReadByte();
                    if (len != Primitives.HashSize)
                    {
                        return new ResponseBlock() { Success = false, Message = "The hash size is not equal" };
                    }
                    block.PreviousHash = new Primitives.Hash { Value = bin.ReadBytes(len) };
                    block.Sequence = bin.ReadUInt64();

                    // user fields
                    int cnt = (int)bin.ReadByte();
                    if (cnt > 0)
                    {
                        block.UserFields = new List<UserField>();
                        for (int i = 0; i < cnt; i++)
                        {
                            block.UserFields.Add(UserField.Parse(bin));
                        }
                    }

                    // round cost
                    block.RoundCost = new Money();
                    block.RoundCost.Integral = bin.ReadInt32();
                    block.RoundCost.Fraction = bin.ReadUInt64();

                    // transactions
                    cnt = (int)bin.ReadUInt32();
                    if (cnt > 0)
                    {
                        block.Transactions = new List<Transaction>();
                        for (int i = 0; i < cnt; i++)
                        {
                            block.Transactions.Add(Transaction.Parse(bin));
                        }

                        // round cost clarify
                        if (block.RoundCost.IsZero)
                        {
                            foreach (var t in block.Transactions)
                            {
                                block.RoundCost += t.ActualFee;
                            }
                        }
                    }

                    // new wallets
                    cnt = (int)bin.ReadUInt32();
                    if (cnt > 0)
                    {
                        block.IntroducedWallets = new List<WalletIntroduce>();
                        for (int i = 0; i < cnt; i++)
                        {
                            WalletIntroduce w = new WalletIntroduce();
                            Int64 v = bin.ReadInt64();
                            if (v < 0)
                            {
                                v = -v;
                                // address is target
                                if (v < block.Transactions.Count)
                                {
                                    w.Key = block.Transactions[(int)v].Target;
                                }
                            }
                            else
                            {
                                if (v < block.Transactions.Count)
                                {
                                    w.Key = block.Transactions[(int)v].Source;
                                }
                            }
                            w.Id = bin.ReadUInt32();
                            block.IntroducedWallets.Add(w);
                        }
                    }

                    // consensus info

                    // current RT
                    cnt = (int)(uint)bin.ReadByte();
                    UInt64 bits_trusted = bin.ReadUInt64(); // count of "1" = sig_blk_cnt
                    if (cnt > 0)
                    {
                        block.TrustedNodes = new List<ConsensusMember>();
                        for (int i = 0; i < cnt; i++)
                        {
                            block.TrustedNodes.Add(new ConsensusMember()
                            {
                                Id = new Primitives.PublicKey { Value = bin.ReadBytes(Primitives.PublicKeySize) }
                            });
                        }
                    }

                    // previous consensus
                    cnt = (int)(uint)bin.ReadByte();
                    UInt64 bits_appr = bin.ReadUInt64();
                    if (cnt > 0)
                    {
                        block.TrustedApproval = new List<KeyValuePair<int, Primitives.Signature>>();
                        
                        int sig_prev_rt_cnt = CountBits(bits_appr);
                        int isig = 0;
                        for (int i = 0; i < cnt; i++)
                        {
                            if ((bits_appr & (0x1UL << i)) == 0)
                            {
                                continue;
                            }
                            if (isig >= sig_prev_rt_cnt)
                            {
                                break;
                            }
                            block.TrustedApproval.Add(new KeyValuePair<int, Primitives.Signature>(
                                isig,
                                new Primitives.Signature { Value = bin.ReadBytes(Primitives.SignatureSize) }
                            ));
                            isig++;
                        }
                        if (isig != sig_prev_rt_cnt)
                        {
                            //TODO: malformed trusted approval section
                        }
                    }

                    hashing_len = (int)(uint)bin.ReadUInt64();

                    // continue read block.TrustedNodes (signatures)
                    cnt = block.TrustedNodes.Count;
                    for (int i = 0; i < cnt; i++)
                    {
                        if ((bits_trusted & (0x1UL << i)) == 0)
                        {
                            continue;
                        }
                        block.TrustedNodes[i].Signature = new Primitives.Signature() { Value = bin.ReadBytes(Primitives.SignatureSize) };
                    }

                    // contract signatures
                    cnt = (int)(uint)bin.ReadByte();
                    if (cnt > 0)
                    {
                        block.ContractsApproval = new List<ContractConfirmation>();
                        for (int i = 0; i < cnt; i++)
                        {
                            block.ContractsApproval.Add(ContractConfirmation.Parse(bin));
                        }
                    }

                    if (bin.BaseStream.Position == bin.BaseStream.Length)
                    {
                        if (hashing_len > 0)
                        {
                            block.Hash = new Primitives.Hash() { Value = Blake2s.ComputeHash(Primitives.HashSize, bytes.AsSpan(0, hashing_len)) };
                        }
                        return new ResponseBlock() { Success = true, Block = block };
                    }

                    return new ResponseBlock() { Success = false, Message = "The stream postion is not equal to the stream length" };
                }
            }
            catch (Exception ex)
            {
                return new ResponseBlock() { Success = false, Message = ex.Message };
            }
        }
    
        // Brian Kernighan's neat idea which iterates as many times as there are bits set:
        static int CountBits(UInt64 value)
        {
            int count = 0;
            while (value != 0)
            {
                count++;
                value &= value - 1;
            }
            return count;
        }
    }
}
