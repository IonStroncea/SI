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
        private BigInteger e;
        private BigInteger d;
        private BigInteger n;

        internal Rsa(BigInteger e, BigInteger d, BigInteger n)
        {
            this.e = e;
            this.d = d;
            this.n = n;
        }

        public BigInteger[] Encrypt(string message)
        {
            return message.ToCharArray().ToList()
                .ConvertAll(x => BigInteger.ModPow(x, e, n)).ToArray();
        }

        public string Decrypt(BigInteger[] encryptedText)
        {
            var charArray = encryptedText.ToList()
                .ConvertAll(x => (char)BigInteger.ModPow(x, d, n)).ToArray();
            return new string(charArray);
        }
    }
}
