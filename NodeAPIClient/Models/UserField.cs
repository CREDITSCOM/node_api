using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NodeAPIClient.Models
{
    public class UserField
    {
        public UInt32 Key { get; set; }

        public IntegerVariant Integer { get; set; }
        public BytesVariant Bytes { get; set; }
        public MoneyVariant Money { get; set; }

        public class IntegerVariant
        {
            public UInt64 Value { get; set; }
        }

        public class BytesVariant
        {
            public byte[] Value { get; set; }
        }

        public class MoneyVariant
        {
            public Money Value { get; set; }
        }

        internal static UserField Parse(BinaryReader bin)
        {
            UserField uf = new UserField();
            uf.Key = bin.ReadUInt32();
            byte vtype = bin.ReadByte();
            switch(vtype)
            {
                case 1:
                    uf.Integer = new IntegerVariant() { Value = bin.ReadUInt64() };
                    break;
                case 2:
                    {
                        int len = (int) bin.ReadUInt32();
                        if(len > 0)
                        {
                            uf.Bytes = new BytesVariant() { Value = bin.ReadBytes(len) };
                        }
                    }
                    break;
                case 3:
                    uf.Money = new MoneyVariant() { Value = new Money() };
                    uf.Money.Value.Integral = bin.ReadInt32();
                    uf.Money.Value.Fraction = bin.ReadUInt64();
                    break;
                default:
                    break;
            }

            return uf;
        }
    }
}
