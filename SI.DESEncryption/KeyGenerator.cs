using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SI.DESEncryption
{
    public class KeyGenerator
    {
        private string mainKey = "AABB09182736CCDD";
        private string left;
        private string right;
        private List<string> subKeys = new List<string>();

        public string MainKey { get => mainKey; set => mainKey = value; }
        public string Left { get => left; set => left = value; }
        public string Right { get => right; set => right = value; }
        public List<string> SubKeys { get => subKeys; set => subKeys = value; }

        public static void GetDes()
        {
            string generatedkey = ShiftLeft(this.Left, round) + ShiftLeft(this.Right, round);

            // Reduce Key Size To 8 Bit
            char[] temp = generatedkey.ToArray();
            char[] permutatedKey = new char[8];
            for (int i = 0; i < permutatedKey.Length; i++)
            {
                permutatedKey[i] = temp[DesConstants.PC2[i] - 1];
            }
           /// SubKeys.Add(new string(permutatedKey));
        }

        public void InitialPermutation()
        {
            // Permutate Key With PC1 Table
            char[] Key = MainKey.ToArray();
            char[] permutatedKey = new char[Key.Length];
            for (int i = 0; i < permutatedKey.Length; i++)
            {
                permutatedKey[i] = Key[DesConstants.PC1[i] - 1];
            }

            string Pkey = new string(permutatedKey);
            // Divide The Key To Left And Right
            List<string> key = (from Match m in Regex.Matches(Pkey, @"\d{5}") select m.Value).ToList();
            this.Left = key[0];
            this.Right = key[1];
        }


        private string ShiftLeft(string keyHalf, int numberOfBits)
        {
            /*
             * Another Simple Approach 
             * return keyHalf.Substring(numberOfBits - 1, keyHalf.Length - 1) + keyHalf.Substring(0, numberOfBits);
             */
            string ss = keyHalf + keyHalf;
            var result = ss.Substring(numberOfBits, keyHalf.Length); // keyHalf length Not SS !!
            return result;
        }
    }
}
