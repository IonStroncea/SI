using SI.RSAEncryption.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SI.RSAEncryption
{
    public class GCDResult
    {
        public BigInteger D { get; set; }
        public BigInteger X { get; set; }
        public BigInteger Y { get; set; }

        public GCDResult()
        {

        }

        public GCDResult(BigInteger d, BigInteger x, BigInteger y)
        {
            D = d;
            X = x;
            Y = y;
        }
    };

    public static class EncryptionGenerator
    {
        private static int PrimeLimit = 1000;
        private static List<int> primes;

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

        public static Rsa GetRsaEncryption()
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
            return new Rsa(e, d, n);
        }

        static GCDResult ExtendedGCD(BigInteger a, BigInteger b) // Extended Euclid
        {
            if (b == 0)
            {
                return new GCDResult(a, 1, 0);
            }

            var result1 = ExtendedGCD(b, a % b);

            var newT = result1.X - (a / b) * result1.Y;

            return new GCDResult(result1.D, result1.Y, newT);

        }
    }
}
