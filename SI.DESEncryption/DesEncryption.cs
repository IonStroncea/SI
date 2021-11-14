using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SI.DESEncryption
{
    public class DesEncryption
    {
        public string MasterKey { private set; get; }
        public List<string> RoundKeys { private set; get; }


        private DesEncryption(string masterKey)
        {
            this.MasterKey = masterKey;
            this.RoundKeys = new();
            this.GenerateRoundKeys();
        }

        #region GenerationLogic
        private static string GetRandomHexKey(int length = 16)
        {
            var random = new Random();
            var hex = "0123456789ABCDEF";

            var key = "";
            for (int i = 0; i < length; i++)
            {
                key += hex[random.Next(hex.Length)];
            }

            return key;
        }

        public static DesEncryption Get()
        {
            var masterKey = "AABB09182736CCDD";
            return new(masterKey);
        }
        
        private void GenerateRoundKeys()
        {
            // first permutation
            var permutedKey = this.Permutation(DesConstants.PC1, this.MasterKey);

            for (int i = 0; i < 16; i++)
            {
                permutedKey = this.ShiftLeft(permutedKey[0..7], DesConstants.ShiftBits[i])
                    + this.ShiftLeft(permutedKey[7..14], DesConstants.ShiftBits[i]);

                // second key permutation
                var doublePermutedKey = this.Permutation(DesConstants.PC2, permutedKey);
                this.RoundKeys.Add(doublePermutedKey);
            }
        }
        #endregion

        #region Algorithm
        // hexadecimal to binary conversion
        private string HexToBin(string input)
        {
            var n = input.Length * 4;
            input = Convert.ToString(Convert.ToInt64(input, 16), 2);

            while (input.Length < n)
            {
                input = "0" + input;
            }

            return input;
        }

        //  binary to hexadecimal conversion
        private string BinToHex(string input)
        {
            var n = (int)input.Length / 4;
            input = Convert.ToString(Convert.ToInt64(input, 2), 16);

            while (input.Length < n)
            {
                input = "0" + input;
            }

            return input;
        }

        private string Permutation(int[] sequence, string input)
        {
            var output = "";
            input = this.HexToBin(input);
            
            for (int i = 0; i < sequence.Length; i++)
            {
                output += input.ElementAt(sequence[i] - 1);
            }

            output = this.BinToHex(output);
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

            additionalInfo.Add(new("Master Key", this.MasterKey));

            foreach (var (key, index) in this.RoundKeys.Select((key, index) => (key, index)))
            {
                additionalInfo.Add(new($"Round key { index + 1 }", key));
            }

            return additionalInfo;
        }

        public string Encrypt(string plainText)
        {
            // Initial permutation
            plainText = this.Permutation(DesConstants.IP, plainText);

            foreach (var key in this.RoundKeys)
            {
                plainText = this.Round(plainText, key);
            }

            // 32 bits swap
            plainText = plainText[8..16] + plainText[0..8];

            // Final permutation
            plainText = this.Permutation(DesConstants.IP1, plainText);

            return plainText;
        }

        public string Decrypt(string plainText)
        {
            // Initial permutation
            plainText = this.Permutation(DesConstants.IP, plainText);

            foreach (var key in this.RoundKeys.AsEnumerable().Reverse())
            {
                plainText = this.Round(plainText, key);
            }

            // 32 bits swap
            plainText = plainText[8..16] + plainText[0..8];

            // Final permutation
            plainText = this.Permutation(DesConstants.IP1, plainText);

            return plainText;
        }

    }
}
