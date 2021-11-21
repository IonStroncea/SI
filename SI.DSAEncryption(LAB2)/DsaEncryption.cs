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
        public JavaBigInteger Q { get; private set; } 
        public JavaBigInteger P { get; private set; }
        public JavaBigInteger G { get; private set; }
        public JavaBigInteger X { get; private set; }
        public JavaBigInteger Y { get; private set; }

        private DsaEncryption(JavaBigInteger q, JavaBigInteger p, JavaBigInteger g, JavaBigInteger x, JavaBigInteger y)
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
            var q = JavaBigInteger.GenPseudoPrime(80, confidence, random);

            //Alegerea unui alt numar prin p, astfel incat p-1 mod q = 0. p mai este numit modulul prim
            var p = GetP(q);

            //Alegerea unui numar integer g, altfel incat 1 < g < p, g**q mod p = 1 and g = h**((p–1)/q) mod p.
            var g = GetG(p, q);

            //Alegerea unui numar integer, astfel incat 0 < x < q for this.
            var x = GetX(p, q);

            //Calcularea lui y ca g**x mod p.
            var y = g.ModPow(x, p);

            return new DsaEncryption(q, p, g, x, y);
        }

        private static JavaBigInteger GetP(JavaBigInteger q)
        {
            var bitCount = 256;
            JavaBigInteger result;
            JavaBigInteger resultReduced;
            do
            {
                result = JavaBigInteger.GenPseudoPrime(bitCount, confidence, random);
                resultReduced = result - 1;
                result -= (resultReduced % q);
            } while (!result.IsProbablePrime(confidence));

            return result;
        }

        private static JavaBigInteger GetG(JavaBigInteger p, JavaBigInteger q)
        {
            JavaBigInteger result;
            var pReduced = p - 1;
            var qReduced = pReduced / q;

            do
            {
                result = JavaBigInteger.GenPseudoPrime(pReduced.BitCount, confidence, random);
            } while (result >= pReduced && result <= 1);

            return result.ModPow(qReduced, p);
        }

        private static JavaBigInteger GetX(JavaBigInteger p, JavaBigInteger q)
        {
            JavaBigInteger result;

            do
            {
                result = JavaBigInteger.GenPseudoPrime(q.BitCount, confidence, random);
            } while (result <= 0 && result >= q);

            return result;
        }
        #endregion

        #region Algorithm
        private JavaBigInteger GetK()
        {
            JavaBigInteger result;

            do
            {
                result = JavaBigInteger.GenPseudoPrime(this.Q.BitCount, confidence, random);

            } while (result >= this.Q && result <= 0);

            return result;
        }

        private JavaBigInteger GetR(JavaBigInteger k) => this.G.ModPow(k, this.P) % this.Q;

        private JavaBigInteger GetHash(byte[] messageData)
        {
            var hash = SHA1.Create().ComputeHash(messageData);

            return new JavaBigInteger(hash);
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
    var s = k.ModInverse(this.Q) * (hash + this.X * r) % this.Q;
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
            var w = s.ModInverse(this.Q);
            var u1 = (hash * w) % this.Q;
            var u2 = (r * w) % this.Q;
            var v = ((this.G.ModPow(u1, this.P) * this.Y.ModPow(u2, this.P)) % this.P) % this.Q;

            return v == r;        
        }
    }
}
