using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SI.RSAEncryption
{
    public static class KeyGenerator
    {
        const int PrimeLimit = 1000;
        static IEnumerable<int> Primes
        {
            get
            {
                yield return 2;
                List<int> cache = new List<int>() { 2 };

                var primes = Enumerable.Range(3, int.MaxValue - 3)
                    .Where(candidate => candidate % 2 != 0)
                    .Select(candidate => new
                    {
                        Sqrt = (int)Math.Sqrt(candidate),
                        Current = candidate
                    }).Where(candidate => !cache.TakeWhile(c => c <= candidate.Sqrt)
                    .Any(cachedPrime => candidate.Current % cachedPrime == 0))
                    .Select(p => p.Current);

                foreach (var prime in primes)
                {
                    cache.Add(prime);
                    yield return prime;
                }
            }
        }

        static IEnumerable<int> GetTwoDistinctPrimes()
        {
            var result = new List<int>();

            var primes = Primes.Take(PrimeLimit);

            var random = new Random();
            int firstPrime = primes.ElementAt(random.Next(primes.Count()));
            int secondPrime;
            do
            {
                secondPrime = primes.ElementAt(random.Next(primes.Count()));
            } while (firstPrime == secondPrime);

            result.Add(firstPrime);
            result.Add(secondPrime);

            return result;
        }

        public static Rsa GetRsaEncryption()
        {
            var twoPrimes = GetTwoDistinctPrimes();
            var primes = Primes.Take(PrimeLimit).ToList();
            primes.RemoveAll(x => twoPrimes.Contains(x));

            var n = twoPrimes.Aggregate((a, b) => a * b);
            var phi = twoPrimes.Aggregate((a, b) => (a - 1) * (b - 1));

            var random = new Random();

            var primesLesserThatPhi = primes.TakeWhile(x => x != phi);
            var e = primesLesserThatPhi.ElementAt(random.Next(primesLesserThatPhi.Count()));
            var d = GetD(e, phi);
            return new Rsa(e, d, n);
        }


        static BigInteger GetD(BigInteger e, BigInteger phi)
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

        // Euclidean Algorithm
        static BigInteger Gcd(BigInteger a, BigInteger b)
        {
            BigInteger r;
            if (a < 0) a = -a;
            if (b < 0) b = -b;
            if (b > a)
            {
                r = b;
                b = a;
                a = r;
            }
            while (b > 0)
            {
                r = a % b;
                a = b;
                b = r;
            }
            return b;
        }
    }
}
