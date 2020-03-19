using System;
using System.Collections.Generic;
using System.Text;

namespace NodeAPIClient.Models
{
    public class NodeInfo
    {
        public string Id { get; set; }

        public int Platform { get; set; }

        public string Version { get; set; }

        public UInt64 TopBlock { get; set; }

        public UInt64 StartRound { get; set; }

        public UInt64 CurrentRound { get; set; }

        public UInt64 AveRoundMs { get; set; }

        public UInt64 UptimeMs { get; set; }

        public List<string> GrayList { get; set; }

        public List<string> BlackList { get; set; }
    }
}
