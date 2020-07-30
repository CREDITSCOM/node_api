using System;

namespace NodeAPIClient.Models
{
    public class WalletIntroduce
    {
        public Primitives.PublicKey Key { get; set; }

        public UInt32 Id { get; set; }
    }
}
