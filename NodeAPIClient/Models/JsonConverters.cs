using SimpleBase;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static NodeAPIClient.Models.Primitives;

namespace NodeAPIClient.Models
{
    public static class JsonConverters
    {
        public class HashConverter : JsonConverter<Hash>
        {
            public override Hash Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => new Hash() { Value = Primitives.FromHexString(reader.GetString()) };

            public override void Write(
                Utf8JsonWriter writer,
                Hash hash,
                JsonSerializerOptions options) => writer.WriteStringValue(hash.ToString());
        }

        public class PublicKeyConverter : JsonConverter<PublicKey>
        {
            public override PublicKey Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => new PublicKey() { Value = Base58.Bitcoin.Decode(reader.GetString()).ToArray() };

            public override void Write(
                Utf8JsonWriter writer,
                PublicKey key,
                JsonSerializerOptions options) => writer.WriteStringValue(key.ToString());
        }

        public class SignatureConverter : JsonConverter<Signature>
        {
            public override Signature Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => new Signature() { Value = Primitives.FromHexString(reader.GetString()) };

            public override void Write(
                Utf8JsonWriter writer,
                Signature signature,
                JsonSerializerOptions options) => writer.WriteStringValue(signature.ToString());
        }

        public class BytesConverter : JsonConverter<byte[]>
        {
            public override byte[] Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => Primitives.FromHexString(reader.GetString());

            public override void Write(
                Utf8JsonWriter writer,
                byte[] bytes,
                JsonSerializerOptions options) => writer.WriteStringValue(Primitives.ToHexString(bytes));
        }

        public class UserFieldBytesConverter : JsonConverter<UserField.BytesVariant>
        {
            public override UserField.BytesVariant Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => new UserField.BytesVariant() { Value = Primitives.FromHexString(reader.GetString()) };

            public override void Write(
                Utf8JsonWriter writer,
                UserField.BytesVariant bytes,
                JsonSerializerOptions options) => writer.WriteStringValue(Primitives.ToHexString(bytes.Value));
        }

        public class UserFieldMoneyConverter : JsonConverter<UserField.MoneyVariant>
        {
            public override UserField.MoneyVariant Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => new UserField.MoneyVariant() { Value = Money.FromString(reader.GetString()) };

            public override void Write(
                Utf8JsonWriter writer,
                UserField.MoneyVariant money,
                JsonSerializerOptions options) => writer.WriteStringValue(money.Value?.ToString());
        }

        public class UserFieldIntegerConverter : JsonConverter<UserField.IntegerVariant>
        {
            public override UserField.IntegerVariant Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => new UserField.IntegerVariant() { Value = UInt64.Parse(reader.GetString()) };

            public override void Write(
                Utf8JsonWriter writer,
                UserField.IntegerVariant value,
                JsonSerializerOptions options) => writer.WriteStringValue(value.Value.ToString());
        }


        public class MoneyConverter : JsonConverter<Money>
        {
            public override Money Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => Money.FromString(reader.GetString());

            public override void Write(
                Utf8JsonWriter writer,
                Money money,
                JsonSerializerOptions options) => writer.WriteStringValue(money.ToString());
        }

    }
}
