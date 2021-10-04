using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SI.RSAEncryption
{
    public static class EncryptionGenerator
    {
        private static int PrimeLimit = 10000;
        private static List<int> primes;

        static EncryptionGenerator()
        {
            primes = PrimesGenerator.Primes.Take(PrimeLimit).ToList();
        }

        static IEnumerable<int> GetTwoDistinctPrimes()
        {
            var result = new List<int>();

            var random = new Random();
            int firstPrime = primes.ElementAt(random.Next(primes.Count));
            primes.Remove(firstPrime);
            var secondPrime = primes.ElementAt(random.Next(primes.Count));

            result.Add(firstPrime);
            result.Add(secondPrime);

            return result;
        }

        public static Rsa GetRsaEncryption()
        {
            var twoPrimes = GetTwoDistinctPrimes();

            var n = twoPrimes.Aggregate((a, b) => a * b);
            var phi = twoPrimes.Aggregate((a, b) => (a - 1) * (b - 1));

            var random = new Random();

            var primesLesserThatPhi = primes.TakeWhile(x => x != phi).ToList();
            var e = primesLesserThatPhi.ElementAt(random.Next(primesLesserThatPhi.Count));
            var d = ComputeD(e, phi);
            return new Rsa(e, d, n);
        }

        static BigInteger ComputeD(BigInteger e, BigInteger phi)
        {
            for (var i = phi - 1; i > 1; i--)
            {
                var multiply = e * i;
                var result = multiply % phi;
                if (result.IsOne == true)
                {
                    return i;
                }
            }

            throw new Exception("Couldn't get D");
        }
    }
}
