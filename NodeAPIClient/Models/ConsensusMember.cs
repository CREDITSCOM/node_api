using System;
using System.Collections.Generic;
using System.Text;

namespace NodeAPIClient.Models
{
    public class ConsensusMember: ICloneable
    {
        public Primitives.PublicKey Id { get; set; }

        public Primitives.Signature Signature { get; set; }

        public object Clone()
        {
            return new ConsensusMember()
            {
                Id = this.Id,
                Signature = new Primitives.Signature(this.Signature)
            };
        }
    }
}
