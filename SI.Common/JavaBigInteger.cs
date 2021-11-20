using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SI.Common
{
    /// <summary>
    /// This is a JavaJavaBigInteger class. Holds integer that is more than 64-bit (long).
    /// </summary>
    /// <remarks>
    /// This class contains overloaded arithmetic operators(+, -, *, /, %), bitwise operators(&amp;, |) and other
    /// operations that can be done with normal integer. It also contains implementation of various prime test.
    /// This class also contains methods dealing with cryptography such as generating prime number, generating 
    /// a coprime number.
    /// </remarks>
    public class JavaBigInteger
    {
        // maximum length of the JavaBigInteger in uint (4 bytes)
        // change this to suit the required level of precision.
        private const int maxLength = 70;

        // primes smaller than 2000 to test the generated prime number
        public static readonly int[] primesBelow2000 = {
           2,    3,    5,    7,   11,   13,   17,   19,   23,   29,   31,   37,   41,   43,   47,   53,   59,   61,   67,   71,
          73,   79,   83,   89,   97,  101,  103,  107,  109,  113,  127,  131,  137,  139,  149,  151,  157,  163,  167,  173,
         179,  181,  191,  193,  197,  199,  211,  223,  227,  229,  233,  239,  241,  251,  257,  263,  269,  271,  277,  281,
         283,  293,  307,  311,  313,  317,  331,  337,  347,  349,  353,  359,  367,  373,  379,  383,  389,  397,  401,  409,
         419,  421,  431,  433,  439,  443,  449,  457,  461,  463,  467,  479,  487,  491,  499,  503,  509,  521,  523,  541,
         547,  557,  563,  569,  571,  577,  587,  593,  599,  601,  607,  613,  617,  619,  631,  641,  643,  647,  653,  659,
         661,  673,  677,  683,  691,  701,  709,  719,  727,  733,  739,  743,  751,  757,  761,  769,  773,  787,  797,  809,
         811,  821,  823,  827,  829,  839,  853,  857,  859,  863,  877,  881,  883,  887,  907,  911,  919,  929,  937,  941,
         947,  953,  967,  971,  977,  983,  991,  997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069,
        1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193, 1201, 1213, 1217, 1223,
        1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373,
        1381, 1399, 1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
        1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
        1663, 1667, 1669, 1693, 1697, 1699, 1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 1801, 1811,
        1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889, 1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987,
        1993, 1997, 1999 };

        private uint[] data = null;            // stores bytes from the Big Integer
        public int dataLength;                 // number of actual chars used

        /// <summary>
        /// Default constructor for JavaBigInteger of value 0
        /// </summary>
        public JavaBigInteger()
        {
            data = new uint[maxLength];
            dataLength = 1;
        }


        /// <summary>
        /// Constructor (Default value provided by long)
        /// </summary>
        /// <param name="value">Turn the long value into JavaBigInteger type</param>
        public JavaBigInteger(long value)
        {
            data = new uint[maxLength];
            long tempVal = value;

            // copy bytes from long to JavaBigInteger without any assumption of
            // the length of the long datatype
            dataLength = 0;
            while (value != 0 && dataLength < maxLength)
            {
                data[dataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                dataLength++;
            }

            if (tempVal > 0)         // overflow check for +ve value
            {
                if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
                    throw (new ArithmeticException("Positive overflow in constructor."));
            }
            else if (tempVal < 0)    // underflow check for -ve value
            {
                if (value != -1 || (data[dataLength - 1] & 0x80000000) == 0)
                    throw (new ArithmeticException("Negative underflow in constructor."));
            }

            if (dataLength == 0)
                dataLength = 1;
        }


        /// <summary>
        /// Constructor (Default value provided by ulong)
        /// </summary>
        /// <param name="value">Turn the unsigned long value into JavaBigInteger type</param>
        public JavaBigInteger(ulong value)
        {
            data = new uint[maxLength];

            // copy bytes from ulong to JavaBigInteger without any assumption of
            // the length of the ulong datatype
            dataLength = 0;
            while (value != 0 && dataLength < maxLength)
            {
                data[dataLength] = (uint)(value & 0xFFFFFFFF);
                value >>= 32;
                dataLength++;
            }

            if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Positive overflow in constructor."));

            if (dataLength == 0)
                dataLength = 1;
        }


        /// <summary>
        /// Constructor (Default value provided by JavaBigInteger)
        /// </summary>
        /// <param name="bi"></param>
        public JavaBigInteger(JavaBigInteger bi)
        {
            data = new uint[maxLength];

            dataLength = bi.dataLength;

            for (int i = 0; i < dataLength; i++)
                data[i] = bi.data[i];
        }


        /// <summary>
        /// Constructor (Default value provided by a string of digits of the specified base)
        /// </summary>
        /// <example>
        /// To initialize "a" with the default value of 1234 in base 10:
        ///      JavaBigInteger a = new JavaBigInteger("1234", 10)
        /// To initialize "a" with the default value of -0x1D4F in base 16:
        ///      JavaBigInteger a = new JavaBigInteger("-1D4F", 16)
        /// </example>
        /// 
        /// <param name="value">String value in the format of [sign][magnitude]</param>
        /// <param name="radix">The base of value</param>
        public JavaBigInteger(string value, int radix)
        {
            JavaBigInteger multiplier = new JavaBigInteger(1);
            JavaBigInteger result = new JavaBigInteger();
            value = (value.ToUpper()).Trim();
            int limit = 0;

            if (value[0] == '-')
                limit = 1;

            for (int i = value.Length - 1; i >= limit; i--)
            {
                int posVal = (int)value[i];

                if (posVal >= '0' && posVal <= '9')
                    posVal -= '0';
                else if (posVal >= 'A' && posVal <= 'Z')
                    posVal = (posVal - 'A') + 10;
                else
                    posVal = 9999999;       // arbitrary large


                if (posVal >= radix)
                    throw (new ArithmeticException("Invalid string in constructor."));
                else
                {
                    if (value[0] == '-')
                        posVal = -posVal;

                    result = result + (multiplier * posVal);

                    if ((i - 1) >= limit)
                        multiplier = multiplier * radix;
                }
            }

            if (value[0] == '-')     // negative values
            {
                if ((result.data[maxLength - 1] & 0x80000000) == 0)
                    throw (new ArithmeticException("Negative underflow in constructor."));
            }
            else    // positive values
            {
                if ((result.data[maxLength - 1] & 0x80000000) != 0)
                    throw (new ArithmeticException("Positive overflow in constructor."));
            }

            data = new uint[maxLength];
            for (int i = 0; i < result.dataLength; i++)
                data[i] = result.data[i];

            dataLength = result.dataLength;
        }


        /// <summary>
        /// Constructor (Default value provided by an array of bytes of the specified length.)
        /// </summary>
        /// <param name="inData">A list of byte values</param>
        /// <param name="length">Default -1</param>
        /// <param name="offset">Default 0</param>
        public JavaBigInteger(IList<byte> inData, int length = -1, int offset = 0)
        {
            var inLen = length == -1 ? inData.Count - offset : length;

            dataLength = inLen >> 2;

            int leftOver = inLen & 0x3;
            if (leftOver != 0)         // length not multiples of 4
                dataLength++;

            if (dataLength > maxLength || inLen > inData.Count - offset)
                throw (new ArithmeticException("Byte overflow in constructor."));


            data = new uint[maxLength];

            for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
            {
                data[j] = (uint)((inData[offset + i - 3] << 24) + (inData[offset + i - 2] << 16) +
                                 (inData[offset + i - 1] << 8) + inData[offset + i]);
            }

            if (leftOver == 1)
                data[dataLength - 1] = (uint)inData[offset + 0];
            else if (leftOver == 2)
                data[dataLength - 1] = (uint)((inData[offset + 0] << 8) + inData[offset + 1]);
            else if (leftOver == 3)
                data[dataLength - 1] = (uint)((inData[offset + 0] << 16) + (inData[offset + 1] << 8) + inData[offset + 2]);


            if (dataLength == 0)
                dataLength = 1;

            while (dataLength > 1 && data[dataLength - 1] == 0)
                dataLength--;
        }


        /// <summary>
        ///  Constructor (Default value provided by an array of unsigned integers)
        /// </summary>
        /// <param name="inData">Array of unsigned integer</param>
        public JavaBigInteger(uint[] inData)
        {
            dataLength = inData.Length;

            if (dataLength > maxLength)
                throw (new ArithmeticException("Byte overflow in constructor."));

            data = new uint[maxLength];

            for (int i = dataLength - 1, j = 0; i >= 0; i--, j++)
                data[j] = inData[i];

            while (dataLength > 1 && data[dataLength - 1] == 0)
                dataLength--;
        }


        /// <summary>
        /// Cast a type long value to type JavaBigInteger value
        /// </summary>
        /// <param name="value">A long value</param>
        public static implicit operator JavaBigInteger(long value)
        {
            return (new JavaBigInteger(value));
        }


        /// <summary>
        /// Cast a type ulong value to type JavaBigInteger value
        /// </summary>
        /// <param name="value">An unsigned long value</param>
        public static implicit operator JavaBigInteger(ulong value)
        {
            return (new JavaBigInteger(value));
        }


        /// <summary>
        /// Cast a type int value to type JavaBigInteger value
        /// </summary>
        /// <param name="value">An int value</param>
        public static implicit operator JavaBigInteger(int value)
        {
            return (new JavaBigInteger((long)value));
        }


        /// <summary>
        /// Cast a type uint value to type JavaBigInteger value
        /// </summary>
        /// <param name="value">An unsigned int value</param>
        public static implicit operator JavaBigInteger(uint value)
        {
            return (new JavaBigInteger((ulong)value));
        }


        /// <summary>
        /// Overloading of addition operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Result of the addition of 2 JavaBigIntegers</returns>
        public static JavaBigInteger operator +(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            JavaBigInteger result = new JavaBigInteger()
            {
                dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength
            };

            long carry = 0;
            for (int i = 0; i < result.dataLength; i++)
            {
                long sum = (long)bi1.data[i] + (long)bi2.data[i] + carry;
                carry = sum >> 32;
                result.data[i] = (uint)(sum & 0xFFFFFFFF);
            }

            if (carry != 0 && result.dataLength < maxLength)
            {
                result.data[result.dataLength] = (uint)(carry);
                result.dataLength++;
            }

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;


            // overflow check
            int lastPos = maxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }


        /// <summary>
        /// Overloading of the unary ++ operator, which increments JavaBigInteger by 1
        /// </summary>
        /// <param name="bi1">A JavaBigInteger</param>
        /// <returns>Incremented JavaBigInteger</returns>
        public static JavaBigInteger operator ++(JavaBigInteger bi1)
        {
            JavaBigInteger result = new JavaBigInteger(bi1);

            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < maxLength)
            {
                val = (long)(result.data[index]);
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if (index > result.dataLength)
                result.dataLength = index;
            else
            {
                while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                    result.dataLength--;
            }

            // overflow check
            int lastPos = maxLength - 1;

            // overflow if initial value was +ve but ++ caused a sign
            // change to negative.

            if ((bi1.data[lastPos] & 0x80000000) == 0 &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException("Overflow in ++."));
            }
            return result;
        }


        /// <summary>
        /// Overloading of subtraction operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Result of the subtraction of 2 JavaBigIntegers</returns>
        public static JavaBigInteger operator -(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            JavaBigInteger result = new JavaBigInteger()
            {
                dataLength = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength
            };

            long carryIn = 0;
            for (int i = 0; i < result.dataLength; i++)
            {
                long diff;

                diff = (long)bi1.data[i] - (long)bi2.data[i] - carryIn;
                result.data[i] = (uint)(diff & 0xFFFFFFFF);

                if (diff < 0)
                    carryIn = 1;
                else
                    carryIn = 0;
            }

            // roll over to negative
            if (carryIn != 0)
            {
                for (int i = result.dataLength; i < maxLength; i++)
                    result.data[i] = 0xFFFFFFFF;
                result.dataLength = maxLength;
            }

            // fixed in v1.03 to give correct datalength for a - (-b)
            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            // overflow check

            int lastPos = maxLength - 1;
            if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException());
            }

            return result;
        }


        /// <summary>
        /// Overloading of the unary -- operator, decrements JavaBigInteger by 1
        /// </summary>
        /// <param name="bi1">A JavaBigInteger</param>
        /// <returns>Decremented JavaBigInteger</returns>
        public static JavaBigInteger operator --(JavaBigInteger bi1)
        {
            JavaBigInteger result = new JavaBigInteger(bi1);

            long val;
            bool carryIn = true;
            int index = 0;

            while (carryIn && index < maxLength)
            {
                val = (long)(result.data[index]);
                val--;

                result.data[index] = (uint)(val & 0xFFFFFFFF);

                if (val >= 0)
                    carryIn = false;

                index++;
            }

            if (index > result.dataLength)
                result.dataLength = index;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            // overflow check
            int lastPos = maxLength - 1;

            // overflow if initial value was -ve but -- caused a sign
            // change to positive.

            if ((bi1.data[lastPos] & 0x80000000) != 0 &&
               (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
            {
                throw (new ArithmeticException("Underflow in --."));
            }

            return result;
        }


        /// <summary>
        /// Overloading of multiplication operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Result of the multiplication of 2 JavaBigIntegers</returns>
        public static JavaBigInteger operator *(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            int lastPos = maxLength - 1;
            bool bi1Neg = false, bi2Neg = false;

            // take the absolute value of the inputs
            try
            {
                if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
                {
                    bi1Neg = true; bi1 = -bi1;
                }
                if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
                {
                    bi2Neg = true; bi2 = -bi2;
                }
            }
            catch (Exception) { }

            JavaBigInteger result = new JavaBigInteger();

            // multiply the absolute values
            try
            {
                for (int i = 0; i < bi1.dataLength; i++)
                {
                    if (bi1.data[i] == 0) continue;

                    ulong mcarry = 0;
                    for (int j = 0, k = i; j < bi2.dataLength; j++, k++)
                    {
                        // k = i + j
                        ulong val = ((ulong)bi1.data[i] * (ulong)bi2.data[j]) +
                                     (ulong)result.data[k] + mcarry;

                        result.data[k] = (uint)(val & 0xFFFFFFFF);
                        mcarry = (val >> 32);
                    }

                    if (mcarry != 0)
                        result.data[i + bi2.dataLength] = (uint)mcarry;
                }
            }
            catch (Exception)
            {
                throw (new ArithmeticException("Multiplication overflow."));
            }


            result.dataLength = bi1.dataLength + bi2.dataLength;
            if (result.dataLength > maxLength)
                result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            // overflow check (result is -ve)
            if ((result.data[lastPos] & 0x80000000) != 0)
            {
                if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000)    // different sign
                {
                    // handle the special case where multiplication produces
                    // a max negative number in 2's complement.

                    if (result.dataLength == 1)
                        return result;
                    else
                    {
                        bool isMaxNeg = true;
                        for (int i = 0; i < result.dataLength - 1 && isMaxNeg; i++)
                        {
                            if (result.data[i] != 0)
                                isMaxNeg = false;
                        }

                        if (isMaxNeg)
                            return result;
                    }
                }

                throw (new ArithmeticException("Multiplication overflow."));
            }

            // if input has different signs, then result is -ve
            if (bi1Neg != bi2Neg)
                return -result;

            return result;
        }


        /// <summary>
        /// Overloading of the unary &lt;&lt; operator (left shift)
        /// </summary>
        /// <remarks>
        /// Shifting by a negative number is an undefined behaviour (UB).
        /// </remarks>
        /// <param name="bi1">A JavaBigInteger</param>
        /// <param name="shiftVal">Left shift by shiftVal bit</param>
        /// <returns>Left-shifted JavaBigInteger</returns>
        public static JavaBigInteger operator <<(JavaBigInteger bi1, int shiftVal)
        {
            JavaBigInteger result = new JavaBigInteger(bi1);
            result.dataLength = ShiftLeft(result.data, shiftVal);

            return result;
        }

        // least significant bits at lower part of buffer
        private static int ShiftLeft(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                    shiftAmount = count;

                ulong carry = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    ulong val = ((ulong)buffer[i]) << shiftAmount;
                    val |= carry;

                    buffer[i] = (uint)(val & 0xFFFFFFFF);
                    carry = val >> 32;
                }

                if (carry != 0)
                {
                    if (bufLen + 1 <= buffer.Length)
                    {
                        buffer[bufLen] = (uint)carry;
                        bufLen++;
                    }
                }
                count -= shiftAmount;
            }
            return bufLen;
        }


        /// <summary>
        /// Overloading of the unary &gt;&gt; operator (right shift)
        /// </summary>
        /// <remarks>
        /// Shifting by a negative number is an undefined behaviour (UB).
        /// </remarks>
        /// <param name="bi1">A JavaBigInteger</param>
        /// <param name="shiftVal">Right shift by shiftVal bit</param>
        /// <returns>Right-shifted JavaBigInteger</returns>
        public static JavaBigInteger operator >>(JavaBigInteger bi1, int shiftVal)
        {
            JavaBigInteger result = new JavaBigInteger(bi1);
            result.dataLength = ShiftRight(result.data, shiftVal);


            if ((bi1.data[maxLength - 1] & 0x80000000) != 0) // negative
            {
                for (int i = maxLength - 1; i >= result.dataLength; i--)
                    result.data[i] = 0xFFFFFFFF;

                uint mask = 0x80000000;
                for (int i = 0; i < 32; i++)
                {
                    if ((result.data[result.dataLength - 1] & mask) != 0)
                        break;

                    result.data[result.dataLength - 1] |= mask;
                    mask >>= 1;
                }
                result.dataLength = maxLength;
            }

            return result;
        }


        private static int ShiftRight(uint[] buffer, int shiftVal)
        {
            int shiftAmount = 32;
            int invShift = 0;
            int bufLen = buffer.Length;

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            for (int count = shiftVal; count > 0;)
            {
                if (count < shiftAmount)
                {
                    shiftAmount = count;
                    invShift = 32 - shiftAmount;
                }

                ulong carry = 0;
                for (int i = bufLen - 1; i >= 0; i--)
                {
                    ulong val = ((ulong)buffer[i]) >> shiftAmount;
                    val |= carry;

                    carry = (((ulong)buffer[i]) << invShift) & 0xFFFFFFFF;
                    buffer[i] = (uint)(val);
                }

                count -= shiftAmount;
            }

            while (bufLen > 1 && buffer[bufLen - 1] == 0)
                bufLen--;

            return bufLen;
        }


        /// <summary>
        /// Overloading of the bit-wise NOT operator (1's complement)
        /// </summary>
        /// <param name="bi1">A JavaBigInteger</param>
        /// <returns>Complemented JavaBigInteger</returns>
        public static JavaBigInteger operator ~(JavaBigInteger bi1)
        {
            JavaBigInteger result = new JavaBigInteger(bi1);

            for (int i = 0; i < maxLength; i++)
                result.data[i] = (uint)(~(bi1.data[i]));

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }


        /// <summary>
        /// Overloading of the NEGATE operator (2's complement)
        /// </summary>
        /// <param name="bi1">A JavaBigInteger</param>
        /// <returns>Negated JavaBigInteger or default JavaBigInteger value if bi1 is 0</returns>
        public static JavaBigInteger operator -(JavaBigInteger bi1)
        {
            // handle neg of zero separately since it'll cause an overflow
            // if we proceed.

            if (bi1.dataLength == 1 && bi1.data[0] == 0)
                return (new JavaBigInteger());

            JavaBigInteger result = new JavaBigInteger(bi1);

            // 1's complement
            for (int i = 0; i < maxLength; i++)
                result.data[i] = (uint)(~(bi1.data[i]));

            // add one to result of 1's complement
            long val, carry = 1;
            int index = 0;

            while (carry != 0 && index < maxLength)
            {
                val = (long)(result.data[index]);
                val++;

                result.data[index] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;

                index++;
            }

            if ((bi1.data[maxLength - 1] & 0x80000000) == (result.data[maxLength - 1] & 0x80000000))
                throw (new ArithmeticException("Overflow in negation.\n"));

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;
            return result;
        }


        /// <summary>
        /// Overloading of equality operator, allows comparing 2 JavaBigIntegers with == operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Boolean result of the comparison</returns>
        public static bool operator ==(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            return bi1.Equals(bi2);
        }


        /// <summary>
        /// Overloading of not equal operator, allows comparing 2 JavaBigIntegers with != operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Boolean result of the comparison</returns>
        public static bool operator !=(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            return !(bi1.Equals(bi2));
        }


        /// <summary>
        /// Overriding of Equals method, allows comparing JavaBigInteger with an arbitary object
        /// </summary>
        /// <param name="o">Input object, to be casted into JavaBigInteger type for comparison</param>
        /// <returns>Boolean result of the comparison</returns>
        public override bool Equals(object o)
        {
            JavaBigInteger bi = (JavaBigInteger)o;

            if (this.dataLength != bi.dataLength)
                return false;

            for (int i = 0; i < this.dataLength; i++)
            {
                if (this.data[i] != bi.data[i])
                    return false;
            }
            return true;
        }


        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }


        /// <summary>
        /// Overloading of greater than operator, allows comparing 2 JavaBigIntegers with &gt; operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Boolean result of the comparison</returns>
        public static bool operator >(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            int pos = maxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
                return false;

            // bi1 is positive, bi2 is negative
            else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
                return true;

            // same sign
            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1.data[pos] > bi2.data[pos])
                    return true;
                return false;
            }
            return false;
        }


        /// <summary>
        /// Overloading of greater than operator, allows comparing 2 JavaBigIntegers with &lt; operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Boolean result of the comparison</returns>
        public static bool operator <(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            int pos = maxLength - 1;

            // bi1 is negative, bi2 is positive
            if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
                return true;

            // bi1 is positive, bi2 is negative
            else if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
                return false;

            // same sign
            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;
            for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--) ;

            if (pos >= 0)
            {
                if (bi1.data[pos] < bi2.data[pos])
                    return true;
                return false;
            }
            return false;
        }


        /// <summary>
        /// Overloading of greater than or equal to operator, allows comparing 2 JavaBigIntegers with &gt;= operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Boolean result of the comparison</returns>
        public static bool operator >=(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            return (bi1 == bi2 || bi1 > bi2);
        }


        /// <summary>
        /// Overloading of less than or equal to operator, allows comparing 2 JavaBigIntegers with &lt;= operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>Boolean result of the comparison</returns>
        public static bool operator <=(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            return (bi1 == bi2 || bi1 < bi2);
        }


        //***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has more than 1 digit.
        //
        // Algorithm taken from [1]
        //***********************************************************************
        private static void MultiByteDivide(JavaBigInteger bi1, JavaBigInteger bi2,
                                            JavaBigInteger outQuotient, JavaBigInteger outRemainder)
        {
            uint[] result = new uint[maxLength];

            int remainderLen = bi1.dataLength + 1;
            uint[] remainder = new uint[remainderLen];

            uint mask = 0x80000000;
            uint val = bi2.data[bi2.dataLength - 1];
            int shift = 0, resultPos = 0;

            while (mask != 0 && (val & mask) == 0)
            {
                shift++; mask >>= 1;
            }

            for (int i = 0; i < bi1.dataLength; i++)
                remainder[i] = bi1.data[i];
            ShiftLeft(remainder, shift);
            bi2 = bi2 << shift;

            int j = remainderLen - bi2.dataLength;
            int pos = remainderLen - 1;

            ulong firstDivisorByte = bi2.data[bi2.dataLength - 1];
            ulong secondDivisorByte = bi2.data[bi2.dataLength - 2];

            int divisorLen = bi2.dataLength + 1;
            uint[] dividendPart = new uint[divisorLen];

            while (j > 0)
            {
                ulong dividend = ((ulong)remainder[pos] << 32) + (ulong)remainder[pos - 1];

                ulong q_hat = dividend / firstDivisorByte;
                ulong r_hat = dividend % firstDivisorByte;

                bool done = false;
                while (!done)
                {
                    done = true;

                    if (q_hat == 0x100000000 ||
                       (q_hat * secondDivisorByte) > ((r_hat << 32) + remainder[pos - 2]))
                    {
                        q_hat--;
                        r_hat += firstDivisorByte;

                        if (r_hat < 0x100000000)
                            done = false;
                    }
                }

                for (int h = 0; h < divisorLen; h++)
                    dividendPart[h] = remainder[pos - h];

                JavaBigInteger kk = new JavaBigInteger(dividendPart);
                JavaBigInteger ss = bi2 * (long)q_hat;

                while (ss > kk)
                {
                    q_hat--;
                    ss -= bi2;
                }
                JavaBigInteger yy = kk - ss;

                for (int h = 0; h < divisorLen; h++)
                    remainder[pos - h] = yy.data[bi2.dataLength - h];

                result[resultPos++] = (uint)q_hat;

                pos--;
                j--;
            }

            outQuotient.dataLength = resultPos;
            int y = 0;
            for (int x = outQuotient.dataLength - 1; x >= 0; x--, y++)
                outQuotient.data[y] = result[x];
            for (; y < maxLength; y++)
                outQuotient.data[y] = 0;

            while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
                outQuotient.dataLength--;

            if (outQuotient.dataLength == 0)
                outQuotient.dataLength = 1;

            outRemainder.dataLength = ShiftRight(remainder, shift);

            for (y = 0; y < outRemainder.dataLength; y++)
                outRemainder.data[y] = remainder[y];
            for (; y < maxLength; y++)
                outRemainder.data[y] = 0;
        }


        //***********************************************************************
        // Private function that supports the division of two numbers with
        // a divisor that has only 1 digit.
        //***********************************************************************
        private static void SingleByteDivide(JavaBigInteger bi1, JavaBigInteger bi2,
                                             JavaBigInteger outQuotient, JavaBigInteger outRemainder)
        {
            uint[] result = new uint[maxLength];
            int resultPos = 0;

            // copy dividend to reminder
            for (int i = 0; i < maxLength; i++)
                outRemainder.data[i] = bi1.data[i];
            outRemainder.dataLength = bi1.dataLength;

            while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
                outRemainder.dataLength--;

            ulong divisor = (ulong)bi2.data[0];
            int pos = outRemainder.dataLength - 1;
            ulong dividend = (ulong)outRemainder.data[pos];

            if (dividend >= divisor)
            {
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos] = (uint)(dividend % divisor);
            }
            pos--;

            while (pos >= 0)
            {
                dividend = ((ulong)outRemainder.data[pos + 1] << 32) + (ulong)outRemainder.data[pos];
                ulong quotient = dividend / divisor;
                result[resultPos++] = (uint)quotient;

                outRemainder.data[pos + 1] = 0;
                outRemainder.data[pos--] = (uint)(dividend % divisor);
            }

            outQuotient.dataLength = resultPos;
            int j = 0;
            for (int i = outQuotient.dataLength - 1; i >= 0; i--, j++)
                outQuotient.data[j] = result[i];
            for (; j < maxLength; j++)
                outQuotient.data[j] = 0;

            while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
                outQuotient.dataLength--;

            if (outQuotient.dataLength == 0)
                outQuotient.dataLength = 1;

            while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
                outRemainder.dataLength--;
        }


        /// <summary>
        /// Overloading of division operator
        /// </summary>
        /// <remarks>The dataLength of the divisor's absolute value must be less than maxLength</remarks>
        /// <param name="bi1">Dividend</param>
        /// <param name="bi2">Divisor</param>
        /// <returns>Quotient of the division</returns>
        public static JavaBigInteger operator /(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            JavaBigInteger quotient = new JavaBigInteger();
            JavaBigInteger remainder = new JavaBigInteger();

            int lastPos = maxLength - 1;
            bool divisorNeg = false, dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }
            if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
            {
                bi2 = -bi2;
                divisorNeg = true;
            }

            if (bi1 < bi2)
            {
                return quotient;
            }

            else
            {
                if (bi2.dataLength == 1)
                    SingleByteDivide(bi1, bi2, quotient, remainder);
                else
                    MultiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg != divisorNeg)
                    return -quotient;

                return quotient;
            }
        }


        /// <summary>
        /// Overloading of modulus operator
        /// </summary>
        /// <remarks>The dataLength of the divisor's absolute value must be less than maxLength</remarks>
        /// <param name="bi1">Dividend</param>
        /// <param name="bi2">Divisor</param>
        /// <returns>Remainder of the division</returns>
        public static JavaBigInteger operator %(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            JavaBigInteger quotient = new JavaBigInteger();
            JavaBigInteger remainder = new JavaBigInteger(bi1);

            int lastPos = maxLength - 1;
            bool dividendNeg = false;

            if ((bi1.data[lastPos] & 0x80000000) != 0)     // bi1 negative
            {
                bi1 = -bi1;
                dividendNeg = true;
            }
            if ((bi2.data[lastPos] & 0x80000000) != 0)     // bi2 negative
                bi2 = -bi2;

            if (bi1 < bi2)
            {
                return remainder;
            }

            else
            {
                if (bi2.dataLength == 1)
                    SingleByteDivide(bi1, bi2, quotient, remainder);
                else
                    MultiByteDivide(bi1, bi2, quotient, remainder);

                if (dividendNeg)
                    return -remainder;

                return remainder;
            }
        }


        /// <summary>
        /// Overloading of bitwise AND operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>JavaBigInteger result after performing &amp; operation</returns>
        public static JavaBigInteger operator &(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            JavaBigInteger result = new JavaBigInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = (uint)(bi1.data[i] & bi2.data[i]);
                result.data[i] = sum;
            }

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }


        /// <summary>
        /// Overloading of bitwise OR operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>JavaBigInteger result after performing | operation</returns>
        public static JavaBigInteger operator |(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            JavaBigInteger result = new JavaBigInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = (uint)(bi1.data[i] | bi2.data[i]);
                result.data[i] = sum;
            }

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }


        /// <summary>
        /// Overloading of bitwise XOR operator
        /// </summary>
        /// <param name="bi1">First JavaBigInteger</param>
        /// <param name="bi2">Second JavaBigInteger</param>
        /// <returns>JavaBigInteger result after performing ^ operation</returns>
        public static JavaBigInteger operator ^(JavaBigInteger bi1, JavaBigInteger bi2)
        {
            JavaBigInteger result = new JavaBigInteger();

            int len = (bi1.dataLength > bi2.dataLength) ? bi1.dataLength : bi2.dataLength;

            for (int i = 0; i < len; i++)
            {
                uint sum = (uint)(bi1.data[i] ^ bi2.data[i]);
                result.data[i] = sum;
            }

            result.dataLength = maxLength;

            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
                result.dataLength--;

            return result;
        }

        /// <summary>
        /// Returns a string representing the JavaBigInteger in base 10
        /// </summary>
        /// <returns>string representation of the JavaBigInteger</returns>
        public override string ToString()
        {
            return ToString(10);
        }


        /// <summary>
        /// Returns a string representing the JavaBigInteger in [sign][magnitude] format in the specified radix
        /// </summary>
        /// <example>If the value of JavaBigInteger is -255 in base 10, then ToString(16) returns "-FF"</example>
        /// <param name="radix">Base</param>
        /// <returns>string representation of the JavaBigInteger in [sign][magnitude] format</returns>
        public string ToString(int radix)
        {
            if (radix < 2 || radix > 36)
                throw (new ArgumentException("Radix must be >= 2 and <= 36"));

            string charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string result = "";

            JavaBigInteger a = this;

            bool negative = false;
            if ((a.data[maxLength - 1] & 0x80000000) != 0)
            {
                negative = true;
                try
                {
                    a = -a;
                }
                catch (Exception) { }
            }

            JavaBigInteger quotient = new JavaBigInteger();
            JavaBigInteger remainder = new JavaBigInteger();
            JavaBigInteger biRadix = new JavaBigInteger(radix);

            if (a.dataLength == 1 && a.data[0] == 0)
                result = "0";
            else
            {
                while (a.dataLength > 1 || (a.dataLength == 1 && a.data[0] != 0))
                {
                    SingleByteDivide(a, biRadix, quotient, remainder);

                    if (remainder.data[0] < 10)
                        result = remainder.data[0] + result;
                    else
                        result = charSet[(int)remainder.data[0] - 10] + result;

                    a = quotient;
                }
                if (negative)
                    result = "-" + result;
            }

            return result;
        }


        /// <summary>
        /// Returns a hex string showing the contains of the JavaBigInteger
        /// </summary>
        /// <example>
        /// 1) If the value of JavaBigInteger is 255 in base 10, then ToHexString() returns "FF"
        /// 2) If the value of JavaBigInteger is -255 in base 10, thenToHexString() returns ".....FFFFFFFFFF01", which is the 2's complement representation of -255.
        /// </example>
        /// <returns></returns>
        public string ToHexString()
        {
            string result = data[dataLength - 1].ToString("X");

            for (int i = dataLength - 2; i >= 0; i--)
            {
                result += data[i].ToString("X8");
            }

            return result;
        }


        /// <summary>
        /// Modulo Exponentiation
        /// </summary>
        /// <param name="exp">Exponential</param>
        /// <param name="n">Modulo</param>
        /// <returns>JavaBigInteger result of raising this to the power of exp and then modulo n </returns>
        public JavaBigInteger ModPow(JavaBigInteger exp, JavaBigInteger n)
        {
            if ((exp.data[maxLength - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Positive exponents only."));

            JavaBigInteger resultNum = 1;
            JavaBigInteger tempNum;
            bool thisNegative = false;

            if ((this.data[maxLength - 1] & 0x80000000) != 0)   // negative this
            {
                tempNum = -this % n;
                thisNegative = true;
            }
            else
                tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)

            if ((n.data[maxLength - 1] & 0x80000000) != 0)   // negative n
                n = -n;

            // calculate constant = b^(2k) / m
            JavaBigInteger constant = new JavaBigInteger();

            int i = n.dataLength << 1;
            constant.data[i] = 0x00000001;
            constant.dataLength = i + 1;

            constant = constant / n;
            int totalBits = exp.BitCount;
            int count = 0;

            // perform squaring and multiply exponentiation
            for (int pos = 0; pos < exp.dataLength; pos++)
            {
                uint mask = 0x01;

                for (int index = 0; index < 32; index++)
                {
                    if ((exp.data[pos] & mask) != 0)
                        resultNum = BarrettReduction(resultNum * tempNum, n, constant);

                    mask <<= 1;

                    tempNum = BarrettReduction(tempNum * tempNum, n, constant);


                    if (tempNum.dataLength == 1 && tempNum.data[0] == 1)
                    {
                        if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
                            return -resultNum;
                        return resultNum;
                    }
                    count++;
                    if (count == totalBits)
                        break;
                }
            }

            if (thisNegative && (exp.data[0] & 0x1) != 0)    //odd exp
                return -resultNum;

            return resultNum;
        }


        /// <summary>
        /// Fast calculation of modular reduction using Barrett's reduction
        /// </summary>
        /// <remarks>
        /// Requires x &lt; b^(2k), where b is the base.  In this case, base is 2^32 (uint).
        ///
        /// Reference [4]
        /// </remarks>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <param name="constant"></param>
        /// <returns></returns>
        private JavaBigInteger BarrettReduction(JavaBigInteger x, JavaBigInteger n, JavaBigInteger constant)
        {
            int k = n.dataLength,
                kPlusOne = k + 1,
                kMinusOne = k - 1;

            JavaBigInteger q1 = new JavaBigInteger();

            // q1 = x / b^(k-1)
            for (int i = kMinusOne, j = 0; i < x.dataLength; i++, j++)
                q1.data[j] = x.data[i];
            q1.dataLength = x.dataLength - kMinusOne;
            if (q1.dataLength <= 0)
                q1.dataLength = 1;


            JavaBigInteger q2 = q1 * constant;
            JavaBigInteger q3 = new JavaBigInteger();

            // q3 = q2 / b^(k+1)
            for (int i = kPlusOne, j = 0; i < q2.dataLength; i++, j++)
                q3.data[j] = q2.data[i];
            q3.dataLength = q2.dataLength - kPlusOne;
            if (q3.dataLength <= 0)
                q3.dataLength = 1;


            // r1 = x mod b^(k+1)
            // i.e. keep the lowest (k+1) words
            JavaBigInteger r1 = new JavaBigInteger();
            int lengthToCopy = (x.dataLength > kPlusOne) ? kPlusOne : x.dataLength;
            for (int i = 0; i < lengthToCopy; i++)
                r1.data[i] = x.data[i];
            r1.dataLength = lengthToCopy;


            // r2 = (q3 * n) mod b^(k+1)
            // partial multiplication of q3 and n

            JavaBigInteger r2 = new JavaBigInteger();
            for (int i = 0; i < q3.dataLength; i++)
            {
                if (q3.data[i] == 0) continue;

                ulong mcarry = 0;
                int t = i;
                for (int j = 0; j < n.dataLength && t < kPlusOne; j++, t++)
                {
                    // t = i + j
                    ulong val = ((ulong)q3.data[i] * (ulong)n.data[j]) +
                                 (ulong)r2.data[t] + mcarry;

                    r2.data[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = (val >> 32);
                }

                if (t < kPlusOne)
                    r2.data[t] = (uint)mcarry;
            }
            r2.dataLength = kPlusOne;
            while (r2.dataLength > 1 && r2.data[r2.dataLength - 1] == 0)
                r2.dataLength--;

            r1 -= r2;
            if ((r1.data[maxLength - 1] & 0x80000000) != 0)        // negative
            {
                JavaBigInteger val = new JavaBigInteger();
                val.data[kPlusOne] = 0x00000001;
                val.dataLength = kPlusOne + 1;
                r1 += val;
            }

            while (r1 >= n)
                r1 -= n;

            return r1;
        }


        /// <summary>
        /// Returns gcd(this, bi)
        /// </summary>
        /// <param name="bi"></param>
        /// <returns>Greatest Common Divisor of this and bi</returns>
        public JavaBigInteger Gcd(JavaBigInteger bi)
        {
            JavaBigInteger x;
            JavaBigInteger y;

            if ((data[maxLength - 1] & 0x80000000) != 0)     // negative
                x = -this;
            else
                x = this;

            if ((bi.data[maxLength - 1] & 0x80000000) != 0)     // negative
                y = -bi;
            else
                y = bi;

            JavaBigInteger g = y;

            while (x.dataLength > 1 || (x.dataLength == 1 && x.data[0] != 0))
            {
                g = x;
                x = y % x;
                y = g;
            }

            return g;
        }


        /// <summary>
        /// Populates "this" with the specified amount of random bits
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="rand"></param>
        public void GenRandomBits(int bits, Random rand)
        {
            int dwords = bits >> 5;
            int remBits = bits & 0x1F;

            if (remBits != 0)
                dwords++;

            if (dwords > maxLength || bits <= 0)
                throw (new ArithmeticException("Number of required bits is not valid."));

            byte[] randBytes = new byte[dwords * 4];
            rand.NextBytes(randBytes);

            for (int i = 0; i < dwords; i++)
                data[i] = BitConverter.ToUInt32(randBytes, i * 4);

            for (int i = dwords; i < maxLength; i++)
                data[i] = 0;

            if (remBits != 0)
            {
                uint mask;

                if (bits != 1)
                {
                    mask = (uint)(0x01 << (remBits - 1));
                    data[dwords - 1] |= mask;
                }

                mask = (uint)(0xFFFFFFFF >> (32 - remBits));
                data[dwords - 1] &= mask;
            }
            else
                data[dwords - 1] |= 0x80000000;

            dataLength = dwords;

            if (dataLength == 0)
                dataLength = 1;
        }


        /// <summary>
        /// Populates "this" with the specified amount of random bits (secured version)
        /// </summary>
        /// <param name="bits"></param>
        /// <param name="rng"></param>
        public void GenRandomBits(int bits, RNGCryptoServiceProvider rng)
        {
            int dwords = bits >> 5;
            int remBits = bits & 0x1F;

            if (remBits != 0)
                dwords++;

            if (dwords > maxLength || bits <= 0)
                throw (new ArithmeticException("Number of required bits is not valid."));

            byte[] randomBytes = new byte[dwords * 4];
            rng.GetBytes(randomBytes);

            for (int i = 0; i < dwords; i++)
                data[i] = BitConverter.ToUInt32(randomBytes, i * 4);

            for (int i = dwords; i < maxLength; i++)
                data[i] = 0;

            if (remBits != 0)
            {
                uint mask;

                if (bits != 1)
                {
                    mask = (uint)(0x01 << (remBits - 1));
                    data[dwords - 1] |= mask;
                }

                mask = (uint)(0xFFFFFFFF >> (32 - remBits));
                data[dwords - 1] &= mask;
            }
            else
                data[dwords - 1] |= 0x80000000;

            dataLength = dwords;

            if (dataLength == 0)
                dataLength = 1;
        }


        /// <summary>
        /// Returns the position of the most significant bit in the JavaBigInteger
        /// </summary>
        /// <example>
        /// 1) The result is 1, if the value of JavaBigInteger is 0...0000 0000
        /// 2) The result is 1, if the value of JavaBigInteger is 0...0000 0001
        /// 3) The result is 2, if the value of JavaBigInteger is 0...0000 0010
        /// 4) The result is 2, if the value of JavaBigInteger is 0...0000 0011
        /// 5) The result is 5, if the value of JavaBigInteger is 0...0001 0011
        /// </example>
        /// <returns></returns>
        public int BitCount
        {
            get
            {
                while (dataLength > 1 && data[dataLength - 1] == 0)
                    dataLength--;

                uint value = data[dataLength - 1];
                uint mask = 0x80000000;
                int bits = 32;

                while (bits > 0 && (value & mask) == 0)
                {
                    bits--;
                    mask >>= 1;
                }
                bits += ((dataLength - 1) << 5);

                return bits == 0 ? 1 : bits;
            }
        }
        


        /// <summary>
        /// Probabilistic prime test based on Fermat's little theorem
        /// </summary>
        /// <remarks>
        /// for any a &lt; p (p does not divide a) if
        ///      a^(p-1) mod p != 1 then p is not prime.
        ///
        /// Otherwise, p is probably prime (pseudoprime to the chosen base).
        /// 
        /// This method is fast but fails for Carmichael numbers when the randomly chosen base is a factor of the number.
        /// </remarks>
        /// <param name="confidence">Number of chosen bases</param>
        /// <returns>True if this is a pseudoprime to randomly chosen bases</returns>
        public bool FermatLittleTest(int confidence)
        {
            JavaBigInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;

            int bits = thisVal.BitCount;
            JavaBigInteger a = new JavaBigInteger();
            JavaBigInteger p_sub1 = thisVal - (new JavaBigInteger(1));
            Random rand = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool done = false;

                while (!done)       // generate a < n
                {
                    int testBits = 0;

                    // make sure "a" has at least 2 bits
                    while (testBits < 2)
                        testBits = (int)(rand.NextDouble() * bits);

                    a.GenRandomBits(testBits, rand);

                    int byteLen = a.dataLength;

                    // make sure "a" is not 0
                    if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                JavaBigInteger gcdTest = a.Gcd(thisVal);
                if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
                    return false;

                // calculate a^(p-1) mod p
                JavaBigInteger expResult = a.ModPow(p_sub1, thisVal);

                int resultLen = expResult.dataLength;

                // is NOT prime is a^(p-1) mod p != 1

                if (resultLen > 1 || (resultLen == 1 && expResult.data[0] != 1))
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Probabilistic prime test based on Rabin-Miller's
        /// </summary>
        /// <remarks>
        /// for any p &gt; 0 with p - 1 = 2^s * t
        ///
        /// p is probably prime (strong pseudoprime) if for any a &lt; p,
        /// 1) a^t mod p = 1 or
        /// 2) a^((2^j)*t) mod p = p-1 for some 0 &lt;= j &lt;= s-1
        ///
        /// Otherwise, p is composite.
        /// </remarks>
        /// <param name="confidence">Number of chosen bases</param>
        /// <returns>True if this is a strong pseudoprime to randomly chosen bases</returns>
        public bool RabinMillerTest(int confidence)
        {
            JavaBigInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;


            // calculate values of s and t
            JavaBigInteger p_sub1 = thisVal - (new JavaBigInteger(1));
            int s = 0;

            for (int index = 0; index < p_sub1.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((p_sub1.data[index] & mask) != 0)
                    {
                        index = p_sub1.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            JavaBigInteger t = p_sub1 >> s;

            int bits = thisVal.BitCount;
            JavaBigInteger a = new JavaBigInteger();
            Random rand = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool done = false;

                while (!done)       // generate a < n
                {
                    int testBits = 0;

                    // make sure "a" has at least 2 bits
                    while (testBits < 2)
                        testBits = (int)(rand.NextDouble() * bits);

                    a.GenRandomBits(testBits, rand);

                    int byteLen = a.dataLength;

                    // make sure "a" is not 0
                    if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                JavaBigInteger gcdTest = a.Gcd(thisVal);
                if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
                    return false;

                JavaBigInteger b = a.ModPow(t, thisVal);

                bool result = false;

                if (b.dataLength == 1 && b.data[0] == 1)         // a^t mod p = 1
                    result = true;

                for (int j = 0; result == false && j < s; j++)
                {
                    if (b == p_sub1)         // a^((2^j)*t) mod p = p-1 for some 0 <= j <= s-1
                    {
                        result = true;
                        break;
                    }

                    b = (b * b) % thisVal;
                }

                if (result == false)
                    return false;
            }
            return true;
        }


        /// <summary>
        /// Probabilistic prime test based on Solovay-Strassen (Euler Criterion)
        /// </summary>
        /// <remarks>
        ///  p is probably prime if for any a &lt; p (a is not multiple of p),
        /// a^((p-1)/2) mod p = J(a, p)
        ///
        /// where J is the Jacobi symbol.
        ///
        /// Otherwise, p is composite.
        /// </remarks>
        /// <param name="confidence">Number of chosen bases</param>
        /// <returns>True if this is a Euler pseudoprime to randomly chosen bases</returns>
        public bool SolovayStrassenTest(int confidence)
        {
            JavaBigInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;


            int bits = thisVal.BitCount;
            JavaBigInteger a = new JavaBigInteger();
            JavaBigInteger p_sub1 = thisVal - 1;
            JavaBigInteger p_sub1_shift = p_sub1 >> 1;

            Random rand = new Random();

            for (int round = 0; round < confidence; round++)
            {
                bool done = false;

                while (!done)       // generate a < n
                {
                    int testBits = 0;

                    // make sure "a" has at least 2 bits
                    while (testBits < 2)
                        testBits = (int)(rand.NextDouble() * bits);

                    a.GenRandomBits(testBits, rand);

                    int byteLen = a.dataLength;

                    // make sure "a" is not 0
                    if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                        done = true;
                }

                // check whether a factor exists (fix for version 1.03)
                JavaBigInteger gcdTest = a.Gcd(thisVal);
                if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
                    return false;

                // calculate a^((p-1)/2) mod p

                JavaBigInteger expResult = a.ModPow(p_sub1_shift, thisVal);
                if (expResult == p_sub1)
                    expResult = -1;

                // calculate Jacobi symbol
                JavaBigInteger jacob = Jacobi(a, thisVal);

                // if they are different then it is not prime
                if (expResult != jacob)
                    return false;
            }

            return true;
        }


        /// <summary>
        /// Implementation of the Lucas Strong Pseudo Prime test
        /// </summary>
        /// <remarks>
        /// Let n be an odd number with gcd(n,D) = 1, and n - J(D, n) = 2^s * d
        /// with d odd and s >= 0.
        ///
        /// If Ud mod n = 0 or V2^r*d mod n = 0 for some 0 &lt;= r &lt; s, then n
        /// is a strong Lucas pseudoprime with parameters (P, Q).  We select
        /// P and Q based on Selfridge.
        /// </remarks>
        /// <returns>True if number is a strong Lucus pseudo prime</returns>
        public bool LucasStrongTest()
        {
            JavaBigInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            if (thisVal.dataLength == 1)
            {
                // test small numbers
                if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
                    return false;
                else if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
                    return true;
            }

            if ((thisVal.data[0] & 0x1) == 0)     // even numbers
                return false;

            return LucasStrongTestHelper(thisVal);
        }


        private bool LucasStrongTestHelper(JavaBigInteger thisVal)
        {
            // Do the test (selects D based on Selfridge)
            // Let D be the first element of the sequence
            // 5, -7, 9, -11, 13, ... for which J(D,n) = -1
            // Let P = 1, Q = (1-D) / 4

            long D = 5, sign = -1, dCount = 0;
            bool done = false;

            while (!done)
            {
                int Jresult = JavaBigInteger.Jacobi(D, thisVal);

                if (Jresult == -1)
                    done = true;    // J(D, this) = 1
                else
                {
                    if (Jresult == 0 && Math.Abs(D) < thisVal)       // divisor found
                        return false;

                    if (dCount == 20)
                    {
                        // check for square
                        JavaBigInteger root = thisVal.Sqrt();
                        if (root * root == thisVal)
                            return false;
                    }

                    D = (Math.Abs(D) + 2) * sign;
                    sign = -sign;
                }
                dCount++;
            }

            long Q = (1 - D) >> 2;

            JavaBigInteger p_add1 = thisVal + 1;
            int s = 0;

            for (int index = 0; index < p_add1.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((p_add1.data[index] & mask) != 0)
                    {
                        index = p_add1.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            JavaBigInteger t = p_add1 >> s;

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            JavaBigInteger constant = new JavaBigInteger();

            int nLen = thisVal.dataLength << 1;
            constant.data[nLen] = 0x00000001;
            constant.dataLength = nLen + 1;

            constant = constant / thisVal;

            JavaBigInteger[] lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
            bool isPrime = false;

            if ((lucas[0].dataLength == 1 && lucas[0].data[0] == 0) ||
               (lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
            {
                // u(t) = 0 or V(t) = 0
                isPrime = true;
            }

            for (int i = 1; i < s; i++)
            {
                if (!isPrime)
                {
                    // doubling of index
                    lucas[1] = thisVal.BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
                    lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;

                    if ((lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
                        isPrime = true;
                }

                lucas[2] = thisVal.BarrettReduction(lucas[2] * lucas[2], thisVal, constant);     //Q^k
            }


            if (isPrime)     // additional checks for composite numbers
            {
                // If n is prime and gcd(n, Q) == 1, then
                // Q^((n+1)/2) = Q * Q^((n-1)/2) is congruent to (Q * J(Q, n)) mod n

                JavaBigInteger g = thisVal.Gcd(Q);
                if (g.dataLength == 1 && g.data[0] == 1)         // gcd(this, Q) == 1
                {
                    if ((lucas[2].data[maxLength - 1] & 0x80000000) != 0)
                        lucas[2] += thisVal;

                    JavaBigInteger temp = (Q * JavaBigInteger.Jacobi(Q, thisVal)) % thisVal;
                    if ((temp.data[maxLength - 1] & 0x80000000) != 0)
                        temp += thisVal;

                    if (lucas[2] != temp)
                        isPrime = false;
                }
            }

            return isPrime;
        }


        /// <summary>
        /// Determines whether a number is probably prime using the Rabin-Miller's test
        /// </summary>
        /// <remarks>
        /// Before applying the test, the number is tested for divisibility by primes &lt; 2000
        /// </remarks>
        /// <param name="confidence">Number of chosen bases</param>
        /// <returns>True if this is probably prime</returns>
        public bool IsProbablePrime(int confidence)
        {
            JavaBigInteger thisVal;
            if ((this.data[maxLength - 1] & 0x80000000) != 0)        // negative
                thisVal = -this;
            else
                thisVal = this;

            // test for divisibility by primes < 2000
            for (int p = 0; p < primesBelow2000.Length; p++)
            {
                JavaBigInteger divisor = primesBelow2000[p];

                if (divisor >= thisVal)
                    break;

                JavaBigInteger resultNum = thisVal % divisor;
                if (resultNum.IntValue() == 0)
                    return false;
            }

            if (thisVal.RabinMillerTest(confidence))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Returns the lowest 4 bytes of the JavaBigInteger as an int
        /// </summary>
        /// <returns>Lowest 4 bytes as integer</returns>
        public int IntValue()
        {
            return (int)data[0];
        }

        /// <summary>
        /// Computes the Jacobi Symbol for 2 JavaBigInteger a and b
        /// </summary>
        /// <remarks>
        /// Algorithm adapted from [3] and [4] with some optimizations
        /// </remarks>
        /// <param name="a">Any JavaBigInteger</param>
        /// <param name="b">Odd JavaBigInteger</param>
        /// <returns>Jacobi Symbol</returns>
        public static int Jacobi(JavaBigInteger a, JavaBigInteger b)
        {
            // Jacobi defined only for odd integers
            if ((b.data[0] & 0x1) == 0)
                throw (new ArgumentException("Jacobi defined only for odd integers."));

            if (a >= b) a %= b;
            if (a.dataLength == 1 && a.data[0] == 0) return 0;  // a == 0
            if (a.dataLength == 1 && a.data[0] == 1) return 1;  // a == 1

            if (a < 0)
            {
                if ((((b - 1).data[0]) & 0x2) == 0)       //if( (((b-1) >> 1).data[0] & 0x1) == 0)
                    return Jacobi(-a, b);
                else
                    return -Jacobi(-a, b);
            }

            int e = 0;
            for (int index = 0; index < a.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((a.data[index] & mask) != 0)
                    {
                        index = a.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    e++;
                }
            }

            JavaBigInteger a1 = a >> e;

            int s = 1;
            if ((e & 0x1) != 0 && ((b.data[0] & 0x7) == 3 || (b.data[0] & 0x7) == 5))
                s = -1;

            if ((b.data[0] & 0x3) == 3 && (a1.data[0] & 0x3) == 3)
                s = -s;

            if (a1.dataLength == 1 && a1.data[0] == 1)
                return s;
            else
                return (s * Jacobi(b % a1, a1));
        }


        /// <summary>
        /// Generates a positive JavaBigInteger that is probably prime
        /// </summary>
        /// <param name="bits">Number of bit</param>
        /// <param name="confidence">Number of chosen bases</param>
        /// <param name="rand">Random object</param>
        /// <returns>A probably prime number</returns>
        public static JavaBigInteger GenPseudoPrime(int bits, int confidence, Random rand)
        {
            JavaBigInteger result = new JavaBigInteger();
            bool done = false;

            while (!done)
            {
                result.GenRandomBits(bits, rand);
                result.data[0] |= 0x01;     // make it odd

                // prime test
                done = result.IsProbablePrime(confidence);
            }
            return result;
        }

        /// <summary>
        /// Returns the modulo inverse of this
        /// </summary>
        /// <remarks>
        /// Throws ArithmeticException if the inverse does not exist.  (i.e. gcd(this, modulus) != 1)
        /// </remarks>
        /// <param name="modulus"></param>
        /// <returns>Modulo inverse of this</returns>
        public JavaBigInteger ModInverse(JavaBigInteger modulus)
        {
            JavaBigInteger[] p = { 0, 1 };
            JavaBigInteger[] q = new JavaBigInteger[2];    // quotients
            JavaBigInteger[] r = { 0, 0 };             // remainders

            int step = 0;

            JavaBigInteger a = modulus;
            JavaBigInteger b = this;

            while (b.dataLength > 1 || (b.dataLength == 1 && b.data[0] != 0))
            {
                JavaBigInteger quotient = new JavaBigInteger();
                JavaBigInteger remainder = new JavaBigInteger();

                if (step > 1)
                {
                    JavaBigInteger pval = (p[0] - (p[1] * q[0])) % modulus;
                    p[0] = p[1];
                    p[1] = pval;
                }

                if (b.dataLength == 1)
                    SingleByteDivide(a, b, quotient, remainder);
                else
                    MultiByteDivide(a, b, quotient, remainder);

                q[0] = q[1];
                r[0] = r[1];
                q[1] = quotient; r[1] = remainder;

                a = b;
                b = remainder;

                step++;
            }
            if (r[0].dataLength > 1 || (r[0].dataLength == 1 && r[0].data[0] != 1))
                throw (new ArithmeticException("No inverse!"));

            JavaBigInteger result = ((p[0] - (p[1] * q[0])) % modulus);

            if ((result.data[maxLength - 1] & 0x80000000) != 0)
                result += modulus;  // get the least positive modulus

            return result;
        }


        /// <summary>
        /// Returns the value of the JavaBigInteger as a byte array
        /// </summary>
        /// <remarks>
        /// The lowest index contains the MSB
        /// </remarks>
        /// <returns>Byte array containing value of the JavaBigInteger</returns>
        public byte[] GetBytes()
        {
            int numBits = BitCount;

            int numBytes = numBits >> 3;
            if ((numBits & 0x7) != 0)
                numBytes++;

            byte[] result = new byte[numBytes];

            int pos = 0;
            uint tempVal, val = data[dataLength - 1];


            if ((tempVal = (val >> 24 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;

            if ((tempVal = (val >> 16 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            else if (pos > 0)
                pos++;

            if ((tempVal = (val >> 8 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            else if (pos > 0)
                pos++;

            if ((tempVal = (val & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            else if (pos > 0)
                pos++;


            for (int i = dataLength - 2; i >= 0; i--, pos += 4)
            {
                val = data[i];
                result[pos + 3] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 2] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 1] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos] = (byte)(val & 0xFF);
            }

            return result;
        }


        /// <summary>
        /// Sets the value of the specified bit to 1
        /// </summary>
        /// <remarks>
        /// The Least Significant Bit position is 0
        /// </remarks>
        /// <param name="bitNum">The position of bit to be changed</param>
        public void SetBit(uint bitNum)
        {
            uint bytePos = bitNum >> 5;             // divide by 32
            byte bitPos = (byte)(bitNum & 0x1F);    // get the lowest 5 bits

            uint mask = (uint)1 << bitPos;
            this.data[bytePos] |= mask;

            if (bytePos >= this.dataLength)
                this.dataLength = (int)bytePos + 1;
        }


        /// <summary>
        /// Sets the value of the specified bit to 0
        /// </summary>
        /// <remarks>
        /// The Least Significant Bit position is 0
        /// </remarks>
        /// <param name="bitNum">The position of bit to be changed</param>
        public void UnsetBit(uint bitNum)
        {
            uint bytePos = bitNum >> 5;

            if (bytePos < this.dataLength)
            {
                byte bitPos = (byte)(bitNum & 0x1F);

                uint mask = (uint)1 << bitPos;
                uint mask2 = 0xFFFFFFFF ^ mask;

                this.data[bytePos] &= mask2;

                if (this.dataLength > 1 && this.data[this.dataLength - 1] == 0)
                    this.dataLength--;
            }
        }


        /// <summary>
        /// Returns a value that is equivalent to the integer square root of this
        /// </summary>
        /// <remarks>
        /// The integer square root of "this" is defined as the largest integer n, such that (n * n) &lt;= this.
        /// Square root of negative integer is an undefined behaviour (UB).
        /// </remarks>
        /// <returns>Integer square root of this</returns>
        public JavaBigInteger Sqrt()
        {
            uint numBits = (uint)this.BitCount;

            if ((numBits & 0x1) != 0)        // odd number of bits
                numBits = (numBits >> 1) + 1;
            else
                numBits = (numBits >> 1);

            uint bytePos = numBits >> 5;
            byte bitPos = (byte)(numBits & 0x1F);

            uint mask;

            JavaBigInteger result = new JavaBigInteger();
            if (bitPos == 0)
                mask = 0x80000000;
            else
            {
                mask = (uint)1 << bitPos;
                bytePos++;
            }
            result.dataLength = (int)bytePos;

            for (int i = (int)bytePos - 1; i >= 0; i--)
            {
                while (mask != 0)
                {
                    // guess
                    result.data[i] ^= mask;

                    // undo the guess if its square is larger than this
                    if ((result * result) > this)
                        result.data[i] ^= mask;

                    mask >>= 1;
                }
                mask = 0x80000000;
            }
            return result;
        }


        /// <summary>
        /// Returns the k_th number in the Lucas Sequence reduced modulo n
        /// </summary>
        /// <remarks>
        /// Uses index doubling to speed up the process.  For example, to calculate V(k),
        /// we maintain two numbers in the sequence V(n) and V(n+1).
        ///
        /// To obtain V(2n), we use the identity
        ///      V(2n) = (V(n) * V(n)) - (2 * Q^n)
        /// To obtain V(2n+1), we first write it as
        ///      V(2n+1) = V((n+1) + n)
        /// and use the identity
        ///      V(m+n) = V(m) * V(n) - Q * V(m-n)
        /// Hence,
        ///      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
        ///                   = V(n+1) * V(n) - Q^n * V(1)
        ///                   = V(n+1) * V(n) - Q^n * P
        ///
        /// We use k in its binary expansion and perform index doubling for each
        /// bit position.  For each bit position that is set, we perform an
        /// index doubling followed by an index addition.  This means that for V(n),
        /// we need to update it to V(2n+1).  For V(n+1), we need to update it to
        /// V((2n+1)+1) = V(2*(n+1))
        ///
        /// This function returns
        /// [0] = U(k)
        /// [1] = V(k)
        /// [2] = Q^n
        ///
        /// Where U(0) = 0 % n, U(1) = 1 % n
        ///       V(0) = 2 % n, V(1) = P % n
        /// </remarks>
        /// <param name="P"></param>
        /// <param name="Q"></param>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static JavaBigInteger[] LucasSequence(JavaBigInteger P, JavaBigInteger Q,
                                                 JavaBigInteger k, JavaBigInteger n)
        {
            if (k.dataLength == 1 && k.data[0] == 0)
            {
                JavaBigInteger[] result = new JavaBigInteger[3];

                result[0] = 0; result[1] = 2 % n; result[2] = 1 % n;
                return result;
            }

            // calculate constant = b^(2k) / m
            // for Barrett Reduction
            JavaBigInteger constant = new JavaBigInteger();

            int nLen = n.dataLength << 1;
            constant.data[nLen] = 0x00000001;
            constant.dataLength = nLen + 1;

            constant = constant / n;

            // calculate values of s and t
            int s = 0;

            for (int index = 0; index < k.dataLength; index++)
            {
                uint mask = 0x01;

                for (int i = 0; i < 32; i++)
                {
                    if ((k.data[index] & mask) != 0)
                    {
                        index = k.dataLength;      // to break the outer loop
                        break;
                    }
                    mask <<= 1;
                    s++;
                }
            }

            JavaBigInteger t = k >> s;

            return LucasSequenceHelper(P, Q, t, n, constant, s);
        }


        //***********************************************************************
        // Performs the calculation of the kth term in the Lucas Sequence.
        // For details of the algorithm, see reference [9].
        //
        // k must be odd.  i.e LSB == 1
        //***********************************************************************
        private static JavaBigInteger[] LucasSequenceHelper(JavaBigInteger P, JavaBigInteger Q,
                                                        JavaBigInteger k, JavaBigInteger n,
                                                        JavaBigInteger constant, int s)
        {
            JavaBigInteger[] result = new JavaBigInteger[3];

            if ((k.data[0] & 0x00000001) == 0)
                throw (new ArgumentException("Argument k must be odd."));

            int numbits = k.BitCount;
            uint mask = (uint)0x1 << ((numbits & 0x1F) - 1);

            // v = v0, v1 = v1, u1 = u1, Q_k = Q^0

            JavaBigInteger v = 2 % n, Q_k = 1 % n,
                       v1 = P % n, u1 = Q_k;
            bool flag = true;

            for (int i = k.dataLength - 1; i >= 0; i--)     // iterate on the binary expansion of k
            {
                while (mask != 0)
                {
                    if (i == 0 && mask == 0x00000001)        // last bit
                        break;

                    if ((k.data[i] & mask) != 0)             // bit is set
                    {
                        // index doubling with addition

                        u1 = (u1 * v1) % n;

                        v = ((v * v1) - (P * Q_k)) % n;
                        v1 = n.BarrettReduction(v1 * v1, n, constant);
                        v1 = (v1 - ((Q_k * Q) << 1)) % n;

                        if (flag)
                            flag = false;
                        else
                            Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

                        Q_k = (Q_k * Q) % n;
                    }
                    else
                    {
                        // index doubling
                        u1 = ((u1 * v) - Q_k) % n;

                        v1 = ((v * v1) - (P * Q_k)) % n;
                        v = n.BarrettReduction(v * v, n, constant);
                        v = (v - (Q_k << 1)) % n;

                        if (flag)
                        {
                            Q_k = Q % n;
                            flag = false;
                        }
                        else
                            Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
                    }

                    mask >>= 1;
                }
                mask = 0x80000000;
            }

            // at this point u1 = u(n+1) and v = v(n)
            // since the last bit always 1, we need to transform u1 to u(2n+1) and v to v(2n+1)

            u1 = ((u1 * v) - Q_k) % n;
            v = ((v * v1) - (P * Q_k)) % n;
            if (flag)
                flag = false;
            else
                Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);

            Q_k = (Q_k * Q) % n;


            for (int i = 0; i < s; i++)
            {
                // index doubling
                u1 = (u1 * v) % n;
                v = ((v * v) - (Q_k << 1)) % n;

                if (flag)
                {
                    Q_k = Q % n;
                    flag = false;
                }
                else
                    Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
            }

            result[0] = u1;
            result[1] = v;
            result[2] = Q_k;

            return result;
        }
    }
}
