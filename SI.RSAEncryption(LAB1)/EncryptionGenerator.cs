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
        private static int PrimeLimit = 100;
        private static List<int> primes;

        static EncryptionGenerator()
        {
            FirstPrimesGeneration();
        }

        static void FirstPrimesGeneration()
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
            var phi = twoPrimes.Aggregate((a, b) => (a - 1) * (b - 1)); // Euler function

            var random = new Random();

            var primesLesserThatPhi = primes.TakeWhile(x => x != phi).ToList();
            var e = primesLesserThatPhi.ElementAt(random.Next(primesLesserThatPhi.Count));
            //var d1 = ComputeD(e, phi);
            var result = ComputeD(e, phi);
            var gCDResult1 = ExtendedGCD(e, phi);
            var gCDResult = ExtendedGCD2(e, phi);
            var gCDResult3 = Extended_GDC(e, phi, true);
            var d = gCDResult.X;
            return new Rsa(e, d, n);
        }

        // tested methods that doesn't work for me
        public static BigInteger[] Extended_GDC(BigInteger a, BigInteger modulus, bool calcOnlyModuloInverse)
        {
            BigInteger x, lastX, b_, y, lastY, a_, quotient, temp, temp2, temp3;
            BigInteger[] result;

            if (modulus == 0) return new BigInteger[] { 1, 0, a };

            // We assure ourselves that the two algorithms below will give good results in any case
            if (a < modulus)
            {
                x = 0; lastX = 1; b_ = modulus;
                y = 1; lastY = 0; a_ = a;
            }
            else
            {
                x = 1; lastX = 0; b_ = a;
                y = 0; lastY = 1; a_ = modulus;
            }

            if (calcOnlyModuloInverse)
            {
                // modulo inverse calculation
                // http://snippets.dzone.com/posts/show/4256
                while (a_ > 0)
                {
                    temp = a_;
                    quotient = b_ / a_;     // If not BigInteger used, then use direct cast to Int32

                    a_ = b_ % temp;
                    b_ = temp;
                    temp = y;

                    y = lastY - quotient * temp;
                    lastY = temp;
                }

                lastY %= modulus;

                if (lastY < 0) lastY = (lastY + modulus) % modulus;
                result = new BigInteger[] { 0, lastY, 0 };
            }
            else
            {
                // Extended euclidian algorithm
                // http://everything2.com/title/Extended+Euclidean+algorithm
                // The only good implementation of the full Extended Euclidian Algorithm that I found
                while (a_ > 1)
                {
                    quotient = b_ / a_;     // If not BigInteger used, then use direct cast to Int32
                    temp = x - quotient * y;
                    temp2 = lastX - quotient * lastY;
                    temp3 = b_ - quotient * a_;

                    x = y; lastX = lastY; b_ = a_;
                    y = temp; lastY = temp2; a_ = temp3;
                }

                if (a_ == 0) result = new BigInteger[] { x, lastX, b_ };
                else result = new BigInteger[] { y, lastY, a_ };
            }

            return result;
        }

        static GCDResult ComputeD(BigInteger a, BigInteger b) // Extended Euclidean
        {
            BigInteger x0 = 1, xn = 1;
            BigInteger y0 = 0, yn = 0;
            BigInteger x1 = 0;
            BigInteger y1 = 1;
            BigInteger r = a % b;
            BigInteger q;

            while (r > 0)
            {
                q = a / b;
                xn = x0 - q * x1;
                yn = y0 - q * y1;

                x0 = x1;
                y0 = y1;
                x1 = xn;
                y1 = yn;
                a = b;
                b = r;
                r = a % b;
            }

            return new GCDResult(xn, yn, b);
        }

        static GCDResult ExtendedGCD(BigInteger a, BigInteger b)
        {
            if (b == 0)
            {
                return new GCDResult(a, 1, 0);
            }

            var result1 = ExtendedGCD(b, a % b);

            var newT = result1.X - (a / b) * result1.Y;

            return new GCDResult(result1.D, result1.Y, newT);

        }

        static GCDResult ExtendedGCD2(BigInteger a, BigInteger b)
        {
            var result = new GCDResult();

            if (b == 0)
            {
                result.D = a;
                result.X = 1;
                result.Y = 0;
                return result;
            }
            BigInteger t, t2;
            result = ExtendedGCD(b, a % b);
            t = result.X;
            t2 = result.Y;
            result.X = t2;
            result.Y = t - a / b * t2;
            return result;
        }
    }
}
