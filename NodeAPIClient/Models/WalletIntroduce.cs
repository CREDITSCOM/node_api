using System;
using System.Collections.Generic;
using System.Text;

namespace NodeAPI.Models
{
    public class WalletIntroduce
    {
        public Primitives.PublicKey Key { get; set; }

        public UInt32 Id { get; set; }
    }
}
