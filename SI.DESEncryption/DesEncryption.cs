using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using SI.Common;

namespace SI.DESEncryption
{
    public class DesEncryption
    {
        public BigInteger MasterKey { private set; get; }
        public List<BigInteger> RoundKeys { private set; get; }


        private DesEncryption(BigInteger masterKey)
        {
            this.MasterKey = masterKey;
            this.RoundKeys = new();
            this.GenerateRoundKeys();
        }

        #region GenerationLogic
        private static string GetRandomKey(int length = 16)
        {
            var random = new Random();
            var chars = "0123456789ABCDEF";

            var key = "";
            for (int i = 0; i < length; i++)
            {
                key += chars[random.Next(chars.Length)];
            }

            return key;
        }

        public static DesEncryption Get()
        {
            var generatedKey = GetRandomKey();
            BigInteger hash = 1125899906842597;//new Random().NextLong(long.MaxValue);

            for (int i = 0; i < generatedKey.Length; i++)
            {
                hash = 31 * hash + generatedKey[i];
            }

            return new(hash);
        }
        
        private string AddLeadingZeroesUntil(string input, int until)
        {
            while (input.Length < until)
            {
                input = "0" + input;
            }

            return input;
        }

        private void GenerateRoundKeys()
        {
            var binaryKey = this.MasterKey.ToBinaryString();

            binaryKey = this.AddLeadingZeroesUntil(binaryKey, 64);
    
            // first permutation
            var permutedKey = this.Permutation(DesConstants.PC1, binaryKey);

            // Split permuted string in half | 56/2 = 28
            var keyLeft = permutedKey.Substring(0, 28);
            var keyRight = permutedKey.Substring(28);

            // Parse binary strings into integers for shifting
            var keyLeftDecimal = BigInteger.Parse(this.BinToHex(keyLeft), NumberStyles.HexNumber);
            var keyRightDecimal = BigInteger.Parse(this.BinToHex(keyRight), NumberStyles.HexNumber);

            for (int i = 0; i < 16; i++)
            {
                // Perform left shifts according to key shift array
                keyLeftDecimal = keyLeftDecimal << DesConstants.ShiftBits[i];
                keyRightDecimal = keyRightDecimal << DesConstants.ShiftBits[i];

                // Merge the two halves
                var merged = (keyLeftDecimal << 28) + keyRightDecimal;

                // 56-bit merged
                var mergedString = merged.ToBinaryString();

                mergedString = this.AddLeadingZeroesUntil(mergedString, 56);

                // second key permutation
                var doublePermutedKey = this.Permutation(DesConstants.PC2, mergedString);
                var finalKey = BigInteger.Parse(this.BinToHex(doublePermutedKey), NumberStyles.HexNumber);

                this.RoundKeys.Add(finalKey);
            }
        }
        #endregion

        #region Conversions
        private string UtfToBin(string utf)
        {
            var bytes = Encoding.UTF8.GetBytes(utf);
            var binary = "";

            for (int i = 0; i < bytes.Length; i++)
            {
                int value = bytes[i];
                for (int j = 0; j < 8; j++)
                {
                    binary += ((value & 128) == 0 ? 0 : 1);
                    value <<= 1;
                }
            }

            return binary;
        }

        private string BinToUtf(string binary)
        {
            var chipherTextBytes = new byte[binary.Length / 8];

            for (int i = 0; i < chipherTextBytes.Length; i++)
            {
                var temp = binary[0..8];
                var tempByte = Convert.ToByte(temp, 2);
                chipherTextBytes[i] = tempByte;
                binary = binary.Substring(8);
            }

            return Encoding.UTF8.GetString(chipherTextBytes).Trim();
        }

        //  binary to hexadecimal conversion
        private string BinToHex(string input)
        {
            input = Convert.ToString(Convert.ToInt64(input, 2), 16);
            return input;
        }

        // hexadecimal to binary conversion
        private string HexToBin(string input)
        {
            input = Convert.ToString(Convert.ToInt64(input, 16), 2);
            return input;
        }
        #endregion

        #region Algorithm
        private string Permutation(int[] sequence, string input)
        {
            var output = "";
            
            for (int i = 0; i < sequence.Length; i++)
            {
                output += input.ElementAt(sequence[i] - 1);
            }

            return output;
        }

        private string Xor(string a, string b)
        {
            var decimalA = Convert.ToInt64(a, 16);
            var decimalB = Convert.ToInt64(b, 16);

            // Xor
            decimalA = decimalA ^ decimalB;

            a = Convert.ToString(decimalA, 16);

            while (a.Length < b.Length)
            {
                a = "0" + a;
            }

            return a;
        }

        private string ShiftLeft(string input, int numBits)
        {
            var n = input.Length * 4;
            var permutation = new int[n];

            for (int i = 0; i < n - 1; i++)
            {
                permutation[i] = i + 2;
            }

            permutation[n - 1] = 1;

            while (numBits-- > 0)
            {
                input = this.Permutation(permutation, input);
            }

            return input;
        }

        private string Sbox(string input)
        {
            var output = "";
            input = this.HexToBin(input);

            for (int i = 0; i < 48; i += 6)
            {
                var binaryInputSubstring = input[i..(i + 6)];

                var sboxNum = i / 6;
                var row = Convert.ToInt64($"{ binaryInputSubstring.ElementAt(0) }{ binaryInputSubstring.ElementAt(5) }", 2);
                var col = Convert.ToInt64(binaryInputSubstring[1..5], 2);

                output += Convert.ToString(DesConstants.Sboxes[sboxNum - 1][row, col], 16);
            }

            return output;
        }

        private string Round(string input, string key)
        {
            var left = input[0..8];
            var copiedRight = input[8..16];
            var right = copiedRight;

            // Expansion permutation
            copiedRight = this.Permutation(DesConstants.EP, copiedRight);

            // Xor temp and round key
            copiedRight = this.Xor(copiedRight, key);

            // Sbox
            copiedRight = this.Sbox(copiedRight);

            // Straigth Dbox
            copiedRight = this.Permutation(DesConstants.P, copiedRight);

            left = this.Xor(left, copiedRight);

            // Swapper
            return right + left;
        }
        #endregion


        public List<KeyValuePair<string, string>> GetAdditionalInfo()
        {
            var additionalInfo = new List<KeyValuePair<string, string>>();

            additionalInfo.Add(new("Master Key", this.MasterKey.ToString("X")));

            foreach (var (key, index) in this.RoundKeys.Select((key, index) => (key, index)))
            {
                additionalInfo.Add(new($"Round key { index + 1 }", key.ToString("X")));
            }

            return additionalInfo;
        }

        private string Feistel(string input, string key)
        {
            // Expand function g
            var inputPermuted = this.Permutation(DesConstants.EP, input);

            var inputPermutedDecimal = BigInteger.Parse(this.BinToHex(inputPermuted), NumberStyles.HexNumber);
            var keyDecimal = BigInteger.Parse(this.BinToHex(inputPermuted), NumberStyles.HexNumber);

            // Xor
            var xor = inputPermutedDecimal ^ keyDecimal;

            var binaryXor = xor.ToBinaryString();

            binaryXor = this.AddLeadingZeroesUntil(binaryXor, 48);

            // split into eight 6-bit string
            string[] sin = new string[8];
            for (int i = 0; i < 8; i++)
            {
                sin[i] = binaryXor.Substring(0, 6);
                binaryXor = binaryXor.Substring(6);
            }

            // S-box
            string[] sout = new string[8];
            for (int i = 0; i < 8; i++)
            {
                var row = Convert.ToInt64($"{ sin[i][0] }{ sin[i][5] }", 2);
                var col = Convert.ToInt64(sin[i][1..5], 2);

                sout[i] = Convert.ToString(DesConstants.Sboxes[i][row, col], 2);

                sout[i] = this.AddLeadingZeroesUntil(sout[i], 4);
            }

            // merge s-box outputs
            var merged = "";
            for (int i = 0; i < 8; i++)
            {
                merged = merged + sout[i];
            }

            var result = this.Permutation(DesConstants.P, merged);
            return result;

        }

        public string Encrypt(string plainText)
        {
            var binPlainText = this.UtfToBin(plainText);

            var remainder = binPlainText.Length % 64;
            if (remainder != 0)
            {
                for (int i = 0; i < (64 - remainder); i++)
                {
                    binPlainText = "0" + binPlainText;
                }
            }

            // Separate binary plain text into blocks
            string[] binPlainTextBlocks = new string[binPlainText.Length / 64];
            int offset = 0;
            for (int i = 0; i < binPlainTextBlocks.Length; i++)
            {
                binPlainTextBlocks[i] = binPlainText[offset..(offset + 64)];
                offset += 64;
            }

            string[] binCipherTextBlocks = new string[binPlainText.Length / 64];

            // Encrypt the blocks
            for (int i = 0; i < binCipherTextBlocks.Length; i++)
            {
                binCipherTextBlocks[i] = EncryptBlock(binPlainTextBlocks[i]);
            }

            var binChipherText = "";
            for (int i = 0; i < binCipherTextBlocks.Length; i++)
            {
                binChipherText += binCipherTextBlocks[i];
            }

            return binChipherText;
        }

        private string EncryptBlock(string plainTextBlock)
        {
            if (plainTextBlock.Length != 64) throw new("Input block is not 64 bits!");

            // Initial permutation
            var permutedBlock = this.Permutation(DesConstants.IP, plainTextBlock);

            var left = permutedBlock.Substring(0, 32);
            var right = permutedBlock.Substring(32);

            for (int i = 0; i < 16; i++)
            {
                var currentKey = RoundKeys[i].ToBinaryString();

                currentKey = this.AddLeadingZeroesUntil(currentKey, 48);

                // Get 32-bit result from f
                var fResult = this.Feistel(right, currentKey);

                // Xor
                var f = BigInteger.Parse(this.BinToHex(fResult), NumberStyles.HexNumber);
                var cmL = BigInteger.Parse(this.BinToHex(left), NumberStyles.HexNumber);

                var newRight = cmL ^ f;
                var newRightBinary = newRight.ToBinaryString();

                newRightBinary = this.AddLeadingZeroesUntil(newRightBinary, 32);

                left = right;
                right = newRightBinary;
            }

            var lastPermutation = this.Permutation(DesConstants.IP1, right + left);

            return lastPermutation;
        }

        public string Decrypt(string plainBinText)
        {
            var binPlainText = plainBinText;

            var remainder = binPlainText.Length % 64;
            if (remainder != 0)
            {
                for (int i = 0; i < (64 - remainder); i++)
                {
                    binPlainText = "0" + binPlainText;
                }
            }

            // Separate binary plain text into blocks
            string[] binPlainTextBlocks = new string[binPlainText.Length / 64];
            int offset = 0;
            for (int i = 0; i < binPlainTextBlocks.Length; i++)
            {
                binPlainTextBlocks[i] = binPlainText[offset..(offset + 64)];
                offset += 64;
            }

            string[] binCipherTextBlocks = new string[binPlainText.Length / 64];

            // Encrypt the blocks
            for (int i = 0; i < binCipherTextBlocks.Length; i++)
            {
                binCipherTextBlocks[i] = DecryptBlock(binPlainTextBlocks[i]);
            }

            var binChipherText = "";
            for (int i = 0; i < binCipherTextBlocks.Length; i++)
            {
                binChipherText += binCipherTextBlocks[i];
            }

            return this.BinToUtf(binChipherText);
        }

        private string DecryptBlock(string plainTextBlock)
        {
            if (plainTextBlock.Length != 64) throw new("Input block is not 64 bits!");

            // Initial permutation
            var permutedBlock = this.Permutation(DesConstants.IP, plainTextBlock);

            var left = permutedBlock.Substring(0, 32);
            var right = permutedBlock.Substring(32);

            for (int i = 15; i >= 0; i--)
            {
                var currentKey = RoundKeys[i].ToBinaryString();

                currentKey = this.AddLeadingZeroesUntil(currentKey, 48);

                // Get 32-bit result from f
                var fResult = this.Feistel(right, currentKey);

                // Xor
                var f = BigInteger.Parse(this.BinToHex(fResult), NumberStyles.HexNumber);
                var cmL = BigInteger.Parse(this.BinToHex(left), NumberStyles.HexNumber);

                var newRight = cmL ^ f;
                var newRightBinary = newRight.ToBinaryString();

                newRightBinary = this.AddLeadingZeroesUntil(newRightBinary, 32);

                left = right;
                right = newRightBinary;
            }

            var lastPermutation = this.Permutation(DesConstants.IP1, right + left);

            return lastPermutation;
        }
    }
}
