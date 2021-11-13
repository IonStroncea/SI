using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI.DESEncryption
{
    public class Block
    {

        private string left;
        public string Left 
        { 
            get => left; 
            set => left = value; 
        }

        private string right;
        public string Right 
        { 
            get => right; 
            set => right = value; 
        }


        private string copiedRight;
        public string CopiedRight
        {
            get => copiedRight;
            set => copiedRight = value;
        }

        private string plainTextBlock;
        public string PlainTextBlock { 
            get => plainTextBlock; 
            set => plainTextBlock = value; 
        }

        public Block(string plainTextBlock)
        {
            PlainTextBlock = plainTextBlock;
        }

        public void InitialPermutation()
        {
            char[] permutatedBlock = new char[8];
            for (int i = 0; i < permutatedBlock.Length; i++)
            {
                permutatedBlock[i] = this.PlainTextBlock[DesConstants.IP[i] - 1];
            }
            this.PlainTextBlock = new string(permutatedBlock);
            this.Left = PlainTextBlock.Substring(0, 4);
            this.Right = PlainTextBlock.Substring(4);
            this.CopiedRight = this.Right;
        }

        public void ExpandRight()
        {
            char[] temp = Right.ToArray();
            char[] expandedRight = new char[8];
            for (int i = 0; i < expandedRight.Length; i++)
            {
                expandedRight[i] = temp[DesConstants.EP[i] - 1];
            }
            this.Right = new string(expandedRight);
        }
        
        public void XORWithKey(string roundKey)
        {
            char[] temp1 = roundKey.ToArray();
            char[] temp2 = this.Right.ToArray();
            int[] temp3 = new int[8];
            this.Right = null;
            for (int i = 0; i < temp3.Length; i++)
            {
                temp3[i] = temp1[i] ^ temp2[i];
                this.Right += temp3[i].ToString();
            }
        }
    }
}
