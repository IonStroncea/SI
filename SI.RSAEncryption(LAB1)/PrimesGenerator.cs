using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI.RSAEncryption
{
    public static class PrimesGenerator
    {
        public static IEnumerable<int> Primes
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
    }
}
