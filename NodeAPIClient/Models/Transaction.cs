using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NodeAPI.Models
{
    public class Transaction
    {
        public UInt64 InnerID { get; set; }

        public Primitives.PublicKey Source { get; set; }

        public Primitives.PublicKey Target { get; set; }

        public Money Sum { get; set; }

        public Money MaxFee { get; set; }

        public List<UserField> UserFields { get; set; }

        public Primitives.Signature Signature { get; set; }

        public Money Fee { get; set; }

        public Transaction()
        {
            UserFields = new List<UserField>();
        }

        internal static Transaction Parse(BinaryReader bin)
        {
            Transaction t = new Transaction();
            UInt16 lo = bin.ReadUInt16();
            UInt32 hi = bin.ReadUInt32();
            t.InnerID = (((UInt64)hi & 0x3fffffff) << 16) | lo;
            int len = Primitives.PublicKeySize;
            if ((hi & 0x80000000) != 0)
            {
                len = sizeof(UInt32);
            }
            t.Source = new Primitives.PublicKey() { Value = bin.ReadBytes(len) };
            if ((hi & 0x40000000) != 0)
            {
                len = sizeof(UInt32);
            }
            t.Target = new Primitives.PublicKey() { Value = bin.ReadBytes(len) };
            t.Sum = new Money();
            t.Sum.Integral = bin.ReadInt32();
            t.Sum.Fraction = bin.ReadUInt64();
            t.MaxFee = Money.FromCommission(bin.ReadUInt16());
            // skip currency
            bin.ReadByte();
            int cnt = (int)(uint)bin.ReadByte();
            if(cnt > 0)
            {
                t.UserFields = new List<UserField>();
                for(int i = 0; i < cnt; i++)
                {
                    t.UserFields.Add(UserField.Parse(bin));
                }
            }
            t.Signature = new Primitives.Signature() { Value = bin.ReadBytes(Primitives.SignatureSize) };
            t.Fee = Money.FromCommission(bin.ReadUInt16());

            return t;
        }

    }
}
