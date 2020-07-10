using NodeAPIClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Thrift.Protocol;
using Thrift.Transport;
using static NodeAPIClient.Models.Primitives;

namespace NodeAPIClient.Services
{
    public class GetBlockService
    {
        public string RemoteNodeIp {
            get
            {
                return remote_ip;
            }
            set
            {
                if(string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(remote_ip) || !remote_ip.Equals(value))
                {
                    client = null;
                    remote_ip = value;
                }
            }
        }

        public UInt16 RemoteNodePort
        {
            get
            {
                return remote_port;
            }
            set
            {
                if(remote_port != value)
                {
                    client = null;
                    remote_port = value;
                }
            }
        }

        public int RequestTimeout
        {
            get
            {
                return current_timeout;
            }
            set
            {
                if(current_timeout != value)
                {
                    client = null;
                    current_timeout = value;
                }
            }
        }

        public Models.Block GetBlock(UInt64 sequence)
        {
            var bytes = AcquireBlock(sequence);
            if(bytes == null)
            {
                return null;
            }
            return Models.Block.Parse(bytes);
        }

        /// <summary>   Acquire and return the blocks in range passed. Support both ascending and descending order.
        ///             If not all blocks in range are available, returns successfully retrieved subset</summary>
        ///
        /// <remarks>   Aae, 13.03.2020. </remarks>
        ///
        /// <param name="from"> Start block sequence. </param>
        /// <param name="to">   End block sequence. </param>
        ///
        /// <returns>   The successfully retrieved blocks. The order in list is the same as order set by from..to </returns>

        public List<Models.Block> GetBlocksRange(UInt64 from, UInt64 to)
        {
            List<Models.Block> list = new List<Models.Block>();
            if (from == to)
            {
                var b = GetBlock(from);
                if (b != null)
                {
                    list.Add(b);
                }
            }
            else
            {
                bool desc = (from > to);
                UInt64 i = from;
                while(i != to)
                {
                    var b = GetBlock(i);
                    if(b == null)
                    {
                        break;
                    }
                    list.Add(b);
                    Console.WriteLine(b.Sequence.ToString());
                    if(desc)
                    {
                        i--;
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return list;
        }

        public class BlockContent
        {
            /// <summary>   Gets or sets a value indicating whether to include transactions into output. </summary>
            ///
            /// <value> True if transactions, false if not. </value>

            public bool Transactions { get; set; }

            /// <summary>   Gets or sets a value indicating whether to include the consensus information into output. </summary>
            ///
            /// <value> True if consensus information, false if not. </value>

            public bool ConsensusInfo { get; set; }

            /// <summary>   Gets or sets a value indicating whether to include the contracts approval  into output. </summary>
            ///
            /// <value> True if contracts approval, false if not. </value>

            public bool ContractsApproval { get; set; }

            /// <summary>   Gets or sets a value indicating whether to include the hexadecimal values of hashes into output. </summary>
            ///
            /// <value> True if hashes, false if not. </value>

            public bool Hashes { get; set; }

            /// <summary>   Gets or sets a value indicating whether to include the hexadecimal values of signatures into output. </summary>
            ///
            /// <value> True if signatures, false if not. </value>

            public bool Signatures { get; set; }

            public bool All
            {
                get
                {
                    return Transactions && ConsensusInfo && ContractsApproval && Hashes && Signatures;
                }
            }

            public static BlockContent IncludeAll => new BlockContent()
            {
                Transactions = true,
                Signatures = true,
                Hashes = true,
                ContractsApproval = true,
                ConsensusInfo = true
            };

            public static BlockContent IncludeTransactions => new BlockContent()
            {
                Transactions = true,
                ConsensusInfo = false,
                ContractsApproval = false,
                Hashes = false,
                Signatures = false
            };

            public static BlockContent IncludeConsensus => new BlockContent()
            {
                Transactions = false,
                ConsensusInfo = true,
                ContractsApproval = false,
                Hashes = false,
                Signatures = false
            };

            public static BlockContent SkipBinaries => new BlockContent()
            {
                Transactions = true,
                ConsensusInfo = true,
                ContractsApproval = true,
                Signatures = false,
                Hashes = false
            };
        }

        public static string ToJson(List<Models.Block> blocks, BlockContent include, bool FormatIntended)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = FormatIntended,
            };
            options.Converters.Add(new JsonConverters.HashConverter());
            options.Converters.Add(new JsonConverters.PublicKeyConverter());
            options.Converters.Add(new JsonConverters.SignatureConverter());
            options.Converters.Add(new JsonConverters.MoneyConverter());
            options.Converters.Add(new JsonConverters.BytesConverter());
            options.Converters.Add(new JsonConverters.UserFieldBytesConverter());
            options.Converters.Add(new JsonConverters.UserFieldMoneyConverter());
            options.Converters.Add(new JsonConverters.UserFieldIntegerConverter());

            List<Models.Block> json_content;
            if(include.All)
            {
                json_content = blocks;
            }
            else
            {
                json_content = new List<Block>();
                // remove 
                foreach (var src_block in blocks)
                {
                    json_content.Add( GetBlockService.CopyFrom(src_block, include));
                }
            }

            return JsonSerializer.Serialize(json_content, options);
        }

        private static Block CopyFrom(Block src_block, BlockContent include)
        {
            Models.Block b = new Block();
            b.Version = src_block.Version;
            if(include.Hashes)
            {
                b.PreviousHash = (Hash)src_block.PreviousHash.Clone();
                b.Hash = (Hash)src_block.Hash.Clone();
            }
            else
            {
                b.PreviousHash = Hash.Empty;
                b.Hash = Hash.Empty;
            }
            b.Sequence = src_block.Sequence;
            if(src_block.UserFields != null)
            {
                b.UserFields = new List<UserField>();
                foreach(var src in src_block.UserFields)
                {
                    b.UserFields.Add((UserField)src.Clone());
                }
            }
            b.RoundCost = (Money) src_block.RoundCost.Clone();
            b.IntroducedWallets = src_block.IntroducedWallets;
            if(include.Transactions)
            {
                b.Transactions = new List<Transaction>();
                if (src_block.Transactions != null)
                {
                    foreach(var src in src_block.Transactions)
                    {
                        Models.Transaction t = new Transaction()
                        {
                            InnerID = src.InnerID,
                            Fee = src.Fee,
                            MaxFee = (Money) src.MaxFee.Clone(),
                            Source = new PublicKey(src.Source),
                            Target = new PublicKey(src.Target),
                            Sum = src.Sum
                        };
                        if(src.UserFields != null)
                        {
                            t.UserFields = new List<UserField>();
                            foreach (var src_uf in src.UserFields)
                            {
                                t.UserFields.Add((UserField)src_uf.Clone());
                            }
                        }
                        if(include.Signatures)
                        {
                            t.Signature = new Signature(src.Signature);
                        }
                        else if(src.Signature != null)
                        {
                            t.Signature = Signature.Empty;
                        }
                        b.Transactions.Add(t);
                    }
                }
            }
            else if(src_block.Transactions != null)
            {
                b.Transactions = new List<Transaction>();
            }
            if (include.ConsensusInfo)
            {
                b.TrustedNodes = new List<ConsensusMember>();
                if (src_block.TrustedNodes != null)
                {
                    foreach(var src in src_block.TrustedNodes)
                    {
                        Signature s = null;
                        if(include.Signatures)
                        {
                            s = new Signature(src.Signature);
                        }
                        else if(src.Signature != null)
                        {
                            // an empty signature means: there is a signature that skipped due include params
                            s = Signature.Empty;
                        }
                        b.TrustedNodes.Add(new ConsensusMember() {
                            Id = src.Id,
                            Signature = s
                        });
                    }
                }
                b.TrustedApproval = new List<KeyValuePair<int, Signature>>();
                if (src_block.TrustedApproval != null)
                {
                    foreach(var src in src_block.TrustedApproval)
                    {
                        Signature s = null;
                        if(include.Signatures)
                        {
                            s = new Signature(src.Value);
                        }
                        else if(src.Value != null)
                        {
                            s = Signature.Empty;
                        }
                        b.TrustedApproval.Add(new KeyValuePair<int, Signature>(src.Key, s));
                    }
                }
            }
            else
            {
                if(src_block.TrustedNodes != null)
                {
                    b.TrustedNodes = new List<ConsensusMember>();
                }
                if(src_block.TrustedApproval != null)
                {
                    b.TrustedApproval = new List<KeyValuePair<int, Signature>>();
                }
            }
            if (include.ContractsApproval)
            {
                if (src_block.ContractsApproval != null)
                {
                    b.ContractsApproval = new List<ContractConfirmation>();
                    foreach (var src in src_block.ContractsApproval)
                    {
                        ContractConfirmation cc = new ContractConfirmation()
                        {
                            Key = new PublicKey(src.Key),
                            CallSequence = src.CallSequence
                        };
                        if(src.Signatures != null)
                        {
                            cc.Signatures = new List<KeyValuePair<int, Signature>>();
                            foreach(var src_sig in src.Signatures)
                            {
                                Signature s = null;
                                if(include.Signatures)
                                {
                                    s = new Signature(src_sig.Value);
                                }
                                else if(src_sig.Value != null)
                                {
                                    s = Signature.Empty;
                                }
                                cc.Signatures.Add(new KeyValuePair<int, Signature>(src_sig.Key, s));
                            }
                        }
                        b.ContractsApproval.Add(cc);
                    }
                }
            }
            else if(src_block.ContractsApproval != null)
            {
                b.ContractsApproval = new List<ContractConfirmation>();
            }

            return b;
        }

        byte[] AcquireBlock(UInt64 sequence)
        {
            var cl = getClient();
            if(cl == null)
            {
                return null;
            }
            NodeApiExec.PoolGetResult result;
            try
            {
                result = cl.PoolGet((long)sequence);
                if(result.Status.Code != 0)
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result.Pool;            
        }

        NodeApiExec.APIEXEC.Client client = null;

        string remote_ip = "127.0.0.1";
        ushort remote_port = 9070;
        int current_timeout = 60000;

        NodeApiExec.APIEXEC.Client getClient()
        {
            if(client == null)
            {
                client = Api.ClientFactory.CreateExecutorAPIClient(RemoteNodeIp, RemoteNodePort, RequestTimeout);
            }
            return client;
        }


    }
}
