using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SI.RSAEncryption
{
    public class GcdResult
    {
        public BigInteger D { get; set; }
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }

        public GcdResult()
        {

        }

        public GcdResult(BigInteger d, BigInteger x, BigInteger y)
        {
            this.D = d;
            this.X = x;
            this.Y = y;
        }
    };
}
