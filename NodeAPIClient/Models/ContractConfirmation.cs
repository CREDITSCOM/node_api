using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NodeAPIClient.Models
{
    public class ContractConfirmation
    {
        public Primitives.PublicKey Key { get; set; }

        public UInt64 CallSequence { get; set; }

        // trusted index in table -> signature
        public List<KeyValuePair<int, Primitives.Signature>> Signatures { get; set; }

        internal static ContractConfirmation Parse(BinaryReader bin)
        {
            ContractConfirmation cc = new ContractConfirmation();
            cc.Key = new Primitives.PublicKey() { Value = bin.ReadBytes(Primitives.PublicKeySize) };
            cc.CallSequence = bin.ReadUInt64();
            int cnt = (int)(uint)bin.ReadByte();
            if(cnt > 0)
            {
                cc.Signatures = new List<KeyValuePair<int, Primitives.Signature>>();
                for(int i = 0; i < cnt; i++)
                {
                    int idx = (int)(uint) bin.ReadByte();
                    var sig = new Primitives.Signature() { Value = bin.ReadBytes(Primitives.SignatureSize) };
                    cc.Signatures.Add(new KeyValuePair<int, Primitives.Signature>(idx, sig));
                }
            }

            return cc;
        }
    }
}
