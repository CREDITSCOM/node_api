using System;
using System.Collections.Generic;
using System.Text;

namespace NodeAPIClient.Models
{
    public class ConsensusMember
    {
        public Primitives.PublicKey Id { get; set; }

        public Primitives.Signature Signature { get; set; }
    }
}
