using SI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SI.DSAEncryption
{
    public class DsaEncryption
    {
        public BigInteger Q { get; private set; } 
        public BigInteger P { get; private set; }
        public BigInteger G { get; private set; }
        public BigInteger X { get; private set; }
        public BigInteger Y { get; private set; }

        private DsaEncryption(BigInteger q, BigInteger p, BigInteger g, BigInteger x, BigInteger y)
        {
            this.Q = q;
            this.P = p;
            this.G = g;
            this.X = x;
            this.Y = y;
        }

        #region GenerationLogic
        static Random random;
        static int confidence = 10;

        public static DsaEncryption Get()
        {
            random = new Random();

            //Alegerea unui numar prim q, care este numit divizor prim
            var q = BigInteger.genPseudoPrime(80, confidence, random);

            //Alegerea unui alt numar prin p, astfel incat p-1 mod q = 0. p mai este numit modulul prim
            var p = GetP(q);

            //Alegerea unui numar integer g, altfel incat 1 < g < p, g**q mod p = 1 and g = h**((p–1)/q) mod p.
            var g = GetG(p, q);

            //Alegerea unui numar integer, astfel incat 0 < x < q for this.
            var x = GetX(p, q);

            //Calcularea lui y ca g**x mod p.
            var y = g.modPow(x, p);

            return new DsaEncryption(q, p, g, x, y);
        }

        private static BigInteger GetP(BigInteger q)
        {
            var bitCount = 256;
            BigInteger result;
            BigInteger resultReduced;
            do
            {
                result = BigInteger.genPseudoPrime(bitCount, confidence, random);
                resultReduced = result - 1;
                result -= (resultReduced % q);
            } while (!result.isProbablePrime(confidence));

            return result;
        }

        private static BigInteger GetG(BigInteger p, BigInteger q)
        {
            BigInteger result;
            var pReduced = p - 1;
            var qReduced = pReduced / q;

            do
            {
                result = BigInteger.genPseudoPrime(pReduced.bitCount(), confidence, random);
            } while (result >= pReduced && result <= 1);

            return result.modPow(qReduced, p);
        }

        private static BigInteger GetX(BigInteger p, BigInteger q)
        {
            BigInteger result;

            do
            {
                result = BigInteger.genPseudoPrime(q.bitCount(), confidence, random);
            } while (result <= 0 && result >= q);

            return result;
        }
        #endregion

        #region Algorithm
        private BigInteger GetK()
        {
            BigInteger result;

            do
            {
                result = BigInteger.genPseudoPrime(this.Q.bitCount(), confidence, random);

            } while (result >= this.Q && result <= 0);

            return result;
        }

        private BigInteger GetR(BigInteger k) => this.G.modPow(k, this.P) % this.Q;

        private BigInteger GetHash(byte[] messageData)
        {
            var hash = SHA1.Create().ComputeHash(messageData);

            return new BigInteger(hash);
        }
        #endregion

        public List<KeyValuePair<string, string>> GetAdditionalInfo()
        {
            var additionalInfo = new List<KeyValuePair<string, string>>();

            var firstKeyHalf = $"{ this.P }, { this.Q }, { this.G }";

            additionalInfo.Add(new("Private Key",
                $"({ firstKeyHalf }, { this.X })"));

            additionalInfo.Add(new("Public Key",
                $"({ firstKeyHalf }, { this.Y })"));

            return additionalInfo;
        }

        public Signature SignData(string message)
        {
            var messageData = Encoding.ASCII.GetBytes(message);
            var hash = this.GetHash(messageData);
            var k = this.GetK();
            var r = this.GetR(k);
            var s = k.modInverse(this.Q) * (hash + this.X * r) % this.Q;
            return new Signature(r, s);
        }

        public bool Verify(string message, Signature signature)
        {
            var (r, s) = signature;
            var messageData = Encoding.ASCII.GetBytes(message);

            if (r <= 0 || r >= this.Q)
            {
                return false;
            }
            if (s <= 0 || s >= this.Q)
            {
                return false;
            }

            var hash = this.GetHash(messageData);
            var w = s.modInverse(this.Q);
            var u1 = (hash * w) % this.Q;
            var u2 = (r * w) % this.Q;

            var v = ((this.G.modPow(u1, this.P) * this.Y.modPow(u2, this.P)) % this.P) % this.Q;

            return v == r;        
        }
    }
}
