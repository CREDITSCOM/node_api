using System;
using System.Collections.Generic;
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

        public class Hash
        {
            public byte[] Value { get; set; }

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

        }

        public class PublicKey
        {
            public byte [] Value { get; set; }

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
        }

        public class Signature
        {
            public byte[] Value { get; set; }

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
        }

        public static string ToHexString(byte [] bytes)
        {
            if (bytes == null || bytes.Length == 0)
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
            var bytes = new List<byte>();
            for (var i = 0; i < src.Length / 2; i++)
            {
                bytes.Add(Convert.ToByte(src.Substring(i * 2, 2), 16));
            }
            return bytes.ToArray();
        }

    }
}
