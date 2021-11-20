using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI.DSAEncryption
{
    public class Signature
    {
        public BigInteger R { get; set; }
        public BigInteger S { get; set; }

        public Signature(BigInteger r, BigInteger s)
        {
            this.R = r;
            this.S = s;
        }

        internal void Deconstruct(out BigInteger r, out BigInteger s)
        {
            r = this.R;
            s = this.S;
        }
    }
}
