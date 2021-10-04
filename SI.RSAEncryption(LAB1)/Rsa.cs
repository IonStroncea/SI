using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SI.RSAEncryption
{
    public class Rsa
    {
        public BigInteger E { get; private set; } // public key value
        public BigInteger D { get; private set; } // private key value
        public BigInteger N { get; private set; }

        internal Rsa(BigInteger e, BigInteger d, BigInteger n)
        {
            this.E = e;
            this.D = d;
            this.N = n;
        }

        public BigInteger[] Encrypt(string message)
        {
            return message.ToCharArray().ToList()
                .ConvertAll(x => BigInteger.ModPow(x, E, N)).ToArray();
        }

        public string Decrypt(BigInteger[] encryptedText)
        {
            var charArray = encryptedText.ToList()
                .ConvertAll(x => (char)BigInteger.ModPow(x, D, N)).ToArray();
            return new string(charArray);
        }
    }
}
