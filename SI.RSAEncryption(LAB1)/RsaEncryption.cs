using SI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SI.RSAEncryption
{
    public class RsaEncryption
    {
        public BigInteger E { get; private set; } // public key value
        public BigInteger D { get; private set; } // private key value
        public BigInteger N { get; private set; }

        private RsaEncryption(BigInteger e, BigInteger d, BigInteger n)
        {
            this.E = e;
            this.D = d;
            this.N = n;
        }

        #region GenerationLogic
        static int PrimeLimit = 1000;
        static List<int> primes;

        static void GeneratePrimes()
        {
            primes = PrimesGenerator.Primes.Take(PrimeLimit).ToList();
        }

        static IEnumerable<int> GetTwoDistinctPrimes()
        {
            GeneratePrimes();
            var result = new List<int>();

            var random = new Random();
            int firstPrime = primes.ElementAt(random.Next(primes.Count));
            primes.Remove(firstPrime);
            var secondPrime = primes.ElementAt(random.Next(primes.Count));
            primes.Remove(secondPrime);

            result.Add(firstPrime);
            result.Add(secondPrime);

            return result;
        }

        static GcdResult ExtendedGCD(BigInteger a, BigInteger b) // Extended Euclid
        {
            if (b == 0)
            {
                return new GcdResult(a, 1, 0);
            }

            var result1 = ExtendedGCD(b, a % b);

            var newT = result1.X - (a / b) * result1.Y;

            return new GcdResult(result1.D, result1.Y, newT);

        }

        /// <summary>
        /// Generate and returns the encryption method
        /// </summary>
        /// <returns>Encryption object of type RSA</returns>
        public static RsaEncryption Get()
        {
            BigInteger e, d, n;

            do
            {
                var twoPrimes = GetTwoDistinctPrimes();

                n = twoPrimes.Aggregate((a, b) => a * b);
                var phi = twoPrimes.Aggregate((a, b) => (a - 1) * (b - 1)); // Euler function

                var random = new Random();

                var primesLesserThatPhi = primes.TakeWhile(x => x != phi).ToList();
                e = primesLesserThatPhi.ElementAt(random.Next(primesLesserThatPhi.Count));
                d = ExtendedGCD(e, phi).X;
            } while (d < 0);

            return new RsaEncryption(e, d, n);
        }
        #endregion

        public List<KeyValuePair<string, string>> GetAdditionalInfo()
        {
            var additionalInfo = new List<KeyValuePair<string, string>>();

            additionalInfo.Add(new ("Public Key", 
                $"({ this.E }, { this.N })"));
            additionalInfo.Add(new ("Private Key", 
                $"({ this.D }, { this.N })"));

            return additionalInfo;
        }

        public BigInteger[] Encrypt(string message)
        {
            var encryptedMessage = message.ToCharArray().ToList()
                .ConvertAll(x => BigInteger.ModPow(x, this.E, this.N)).ToArray();

            return encryptedMessage;
        }

        public string Decrypt(BigInteger[] ecryptedMessage)
        {
            var charArray = ecryptedMessage.ToList()
                .ConvertAll(x => (char)BigInteger.ModPow(x, this.D, this.N)).ToArray();

            return new string(charArray);
        }
    }
}
