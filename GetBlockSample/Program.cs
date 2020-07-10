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
                RemoteNodeIp =  "165.22.220.8", // do1
                //RemoteNodeIp = "165.22.212.105", // do6
                RemoteNodePort = 9070
            };

            Primitives.Hash stored_hash = null;
            const UInt64 MaxSeq = 30_157_200;
            List<Block> blocks = service.GetBlocksRange(MaxSeq, MaxSeq - 19);
            foreach (var b in blocks)
            {
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

            string jsonString = GetBlockService.ToJson(blocks, GetBlockService.BlockContent.IncludeAll, true);
            System.IO.File.WriteAllText(@"blocks_all.json", jsonString);

            jsonString = GetBlockService.ToJson(blocks, GetBlockService.BlockContent.IncludeConsensus, true);
            System.IO.File.WriteAllText(@"blocks_consensus.json", jsonString);

            jsonString = GetBlockService.ToJson(blocks, GetBlockService.BlockContent.IncludeTransactions, true);
            System.IO.File.WriteAllText(@"blocks_transactions.json", jsonString);

            jsonString = GetBlockService.ToJson(blocks, GetBlockService.BlockContent.SkipBinaries, true);
            System.IO.File.WriteAllText(@"blocks_skip_bin.json", jsonString);

            return;
        }
    }
}
