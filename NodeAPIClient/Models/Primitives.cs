using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SimpleBase;

namespace NodeAPIClient.Models
{
    public class Primitives
    {
        public const int HashSize = 32;
        public const int PublicKeySize = 32;
        public const int SignatureSize = 64;

        public class Hash: ICloneable
        {
            public byte[] Value { get; set; }

            public Hash() { }

            public Hash(Hash src)
            {
                if(src.IsNullOrEmpty())
                {
                    Value = null;
                }
                else
                {
                    Value = src.Value.ToArray();
                }
            }

            public bool IsNullOrEmpty()
            {
                return Primitives.IsNullOrZero(Value);
            }

            public bool EqualTo(Hash other)
            {
                return Primitives.Equal(Value, other.Value);
            }

            public override string ToString()
            {
                return Primitives.ToHexString(Value);
            }

            public object Clone()
            {
                return new Hash(this);
            }

            public static Hash Empty => new Hash() { Value = Array.Empty<byte>() };
        }

        public class PublicKey: ICloneable
        {
            public byte [] Value { get; set; }

            public PublicKey() { }

            public PublicKey(PublicKey src)
            {
                if (src.IsNullOrEmpty())
                {
                    Value = null;
                }
                else
                {
                    Value = src.Value.ToArray();
                }
            }

            public bool IsNullOrEmpty()
            {
                return Primitives.IsNullOrZero(Value);
            }

            public bool EqualTo(PublicKey other)
            {
                return Primitives.Equal(Value, other.Value);
            }

            public override string ToString()
            {
                if (Value == null || Value.Length == 0)
                {
                    return string.Empty;
                }
                return Base58.Bitcoin.Encode(Value);
            }

            public object Clone()
            {
                return new PublicKey(this);
            }

            public static PublicKey Empty => new PublicKey() { Value = Array.Empty<byte>() };
        }

        public class Signature: ICloneable
        {
            public byte[] Value { get; set; }

            public Signature() { }
            public Signature(Signature src)
            {
                if (src.IsNullOrEmpty())
                {
                    Value = null;
                }
                else
                {
                    Value = src.Value.ToArray();
                }
            }

            public bool IsNullOrEmpty()
            {
                return Primitives.IsNullOrZero(Value);
            }

            public bool EqualTo(Signature other)
            {
                return Primitives.Equal(Value, other.Value);
            }

            public override string ToString()
            {
                return Primitives.ToHexString(Value);
            }

            public object Clone()
            {
                return new Signature(this);
            }

            public static Signature Empty => new Signature() { Value = Array.Empty<byte>() };
        }

        public static string ToHexString(byte [] bytes)
        {
            if (bytes == null)
            {
                return "null";
            }
            if (bytes.Length == 0)
            {
                return string.Empty;
            }
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        public static bool IsNullOrZero(byte [] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return true;
            }
            foreach (var b in bytes)
            {
                if (b != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Equal(byte[] lhs, byte[] rhs)
        {
            if(lhs == null && rhs == null)
            {
                return true;
            }
            if(lhs == null || rhs == null)
            {
                return false;
            }
            if(lhs.Length != rhs.Length)
            {
                return false;
            }
            for(int i = 0; i < lhs.Length; i++)
            {
                if(lhs[i] != rhs[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static byte[] FromHexString(string src)
        {
            if (src.Equals("null"))
            {
                return null;
            }
            var bytes = new List<byte>();
            for (var i = 0; i < src.Length / 2; i++)
            {
                bytes.Add(Convert.ToByte(src.Substring(i * 2, 2), 16));
            }
            return bytes.ToArray();
        }

    }
}
