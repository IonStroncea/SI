using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI.DSAEncryption
{
    public class Signature
    {
        public JavaBigInteger R { get; set; }
        public JavaBigInteger S { get; set; }

        public Signature(JavaBigInteger r, JavaBigInteger s)
        {
            this.R = r;
            this.S = s;
        }

        internal void Deconstruct(out JavaBigInteger r, out JavaBigInteger s)
        {
            r = this.R;
            s = this.S;
        }
    }
}
