using System;
using NodeAPIClient.Services;
using NodeAPIClient.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace GetBlockSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            GetBlockService service = new GetBlockService()
            {
                RemoteNodeIp = "165.22.212.105", // do6
                RemoteNodePort = 9070
            };

            List<Block> blocks = new List<Block>();
            Primitives.Hash stored_hash = null;
            const UInt64 MaxSeq = 30_157_200;
            for (UInt64 seq = MaxSeq; seq >= 0; seq--)
            {
                var b = service.GetBlock(seq);
                if (b == null)
                {
                    continue;
                }
                blocks.Add(b);

                for (int i = 0; i < b.TrustedApproval?.Count; i++)
                {
                    if (b.TrustedApproval[i].Value == null || b.TrustedApproval[i].Value.IsNullOrEmpty())
                    {
                        continue;
                    }
                    if (b.TrustedApproval[i].Key != i)
                    {
                        continue;
                    }
                }

                for (int i = 0; i < b.TrustedNodes?.Count; i++)
                {
                    if (b.TrustedNodes[i].Signature == null || b.TrustedNodes[i].Signature.IsNullOrEmpty())
                    {
                        continue;
                    }
                }

                for (int i = 0; i < b.ContractsApproval.Count; i++)
                {
                    for (int j = 0; j < b.ContractsApproval[i].Signatures?.Count; j++)
                    {
                        if (b.ContractsApproval[i].Signatures[j].Key != j)
                        {
                            continue;
                        }
                        if (b.ContractsApproval[i].Signatures[j].Value == null)
                        {
                            continue;
                        }
                        if (b.ContractsApproval[i].Signatures[j].Value.IsNullOrEmpty())
                        {
                            continue;
                        }
                    }
                }

                if (stored_hash?.Value != null)
                {
                    if (!stored_hash.EqualTo(b.Hash))
                    {
                        continue;
                    }
                }

                stored_hash = b.PreviousHash;
                Console.WriteLine(b.Sequence.ToString());

                if (blocks.Count > 4)
                {
                    break;
                }
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            options.Converters.Add(new JsonConverters.HashConverter());
            options.Converters.Add(new JsonConverters.PublicKeyConverter());
            options.Converters.Add(new JsonConverters.SignatureConverter());
            options.Converters.Add(new JsonConverters.MoneyConverter());
            options.Converters.Add(new JsonConverters.BytesConverter());
            options.Converters.Add(new JsonConverters.UserFieldBytesConverter());
            options.Converters.Add(new JsonConverters.UserFieldMoneyConverter());
            options.Converters.Add(new JsonConverters.UserFieldIntegerConverter());

            string jsonString;
            jsonString = JsonSerializer.Serialize(blocks, options);

            System.IO.File.WriteAllText(@"blocks.json", jsonString);

            //JsonSerializer serializer = new JsonSerializer();
            //serializer.Converters.Add(new JavaScriptDateTimeConverter());
            //serializer.NullValueHandling = NullValueHandling.Ignore;

            //using (StreamWriter sw = new StreamWriter(@"c:\json.txt"))
            //using (JsonWriter writer = new JsonTextWriter(sw))
            //{
            //    serializer.Serialize(writer, product);
            //    // {"ExpiryDate":new Date(1230375600000),"Price":0}
            //}

            return;
        }
    }
}
