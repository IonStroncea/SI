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
        public long MasterKey { private set; get; }
        public List<long> RoundKeys { private set; get; }


        private DesEncryption(string masterKey)
        {
            this.MasterKey = this.StringToLongList(masterKey)[0];
            this.RoundKeys = new();
            this.GenerateRoundKeys();
        }

        #region Conversions
        public static List<byte> LongListToByteList(List<long> valueAsLongList)
        {
            var result = new List<byte>();

            for (int i = 0; i < valueAsLongList.Count; i++)
            {
                for (int t = 7; t >= 0; t--)
                {
                    var b = (byte)(valueAsLongList[i] >> (8 * t));
                    result.Add(b);
                }
            }
            return result;
        }

        private List<long> StringToLongList(string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            var paddedBytes = (bytes.Length % 8 == 0) ? new byte[bytes.Length] : new byte[bytes.Length + 8 - (bytes.Length % 8)];
            
            Array.Copy(bytes, paddedBytes, bytes.Length);
            
            /*
            for (int i = 0; i < bytes.Length; i++)
            {
                paddedBytes[i] = bytes[i];
            }
            */
                
            var longs = new List<long>();
            for (int i = 0; i <= paddedBytes.Length - 8; i += 8)
            {
                var res = this.ByteArrayToLong(paddedBytes, i);
                longs.Add(res);
            }

            return longs;
        }

        private long ByteArrayToLong(byte[] byteArray, int i)
        {
            var l = 0L;
            if (byteArray.Length - i < 8) throw new Exception();
            
            for (int t = 0; t < 8; t++)
            {
                l += (long)byteArray[i + t] << (8 * (8 - t - 1));
            }
            
            return l;
        }
        #endregion

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
            var generatedKey = GetRandomKey(); //"52D57458";
            return new(generatedKey);
        }

        private void GenerateRoundKeys()
        {
            var key56 = this.ApplyPermutation(DesConstants.PC1, this.MasterKey);

            var shiftedKey = key56;
            var key48 = 0L;

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < DesConstants.ShiftBits[i]; j++)
                {
                    shiftedKey = this.ShiftKeyHalvesLeft(shiftedKey);
                }

                key48 = this.ApplyPermutation(DesConstants.PC2, shiftedKey);
                this.RoundKeys.Add(key48);
            }
        }

        private long ShiftKeyHalvesLeft(long key)
        {
            //Look what the bit values in the heads are, which have to circle around
            var leftHead = (byte)((key >> 63) & 1) == 1;
            // 63 - 28 = 35
            var rightHead = (byte)((key >> 35) & 1) == 1;
            // Shift the key 1 bit to the left
            key <<= 1;
            // Set the tails to 0
            // Since we cannot set them to 0 directly, we flip all the bits, then set them to 1 with bitwise or operator, then flip again
            key = ~key;
            key |= ((long)1 << 36);
            key |= ((long)1 << 8);
            key = ~key;
            // Place the head values on the tail
            if (leftHead) key |= (long)1 << 36;
            if (rightHead) key |= (long)1 << 8;

            return key;
        }

        #endregion

        #region Algorithm
        private long ApplyPermutation(byte[] sequence, long value)
        {
            var result = 0L;
            
            for (int i = 0; i < sequence.Length; i++)
            {
                result |= ((value >> (63 - sequence[i] + 1)) & 1) << (63 - i);
            }

            return result;
        }

        private long ApplySBoxes(long value)
        {
            var result = 0L;

            for (int i = 58, sBoxNum = 0; i >= 16; i -= 6, sBoxNum++)
            {
                var sbox = this.GetSBox(value >> i, sBoxNum);
                result |= ((long)sbox << (i + 2 * sBoxNum + 2));
            }

            return result;
        }

        private byte GetSBox(long value, int sBoxNum)
        {
            // look at the rightmost and leftmost bits
            // do not move the leftmost bit 5 positions, but 4 so it is in the second spot.
            var row = (((int)value & 32) >> 4) + ((int)value & 1);
            // Move the 4 center bits on to the right, and despose of the leftmost one.
            var col = ((int)value >> 1) & 15;
            return DesConstants.SBoxes[sBoxNum][row, col];
        }

        private long GetLeftHalf(long value) => (value >> 32) << 32;

        private long GetRightHalf(long value) => (value << 32);

        private long Feistel(long value, bool reverseKeys = false)
        {
            var localKeys = reverseKeys ? 
                this.RoundKeys.AsEnumerable().Reverse() : this.RoundKeys;
            var appliedPermutation = this.ApplyPermutation(DesConstants.IP, value);

            foreach (var key in localKeys)
            {
                var left = this.GetLeftHalf(appliedPermutation);
                var right = this.GetRightHalf(appliedPermutation);
                var newRight = left ^ this.CalculateNewRight(right, key);
                // we only want the rightmost 32 bits, bitwise & with the rightmost 32 bits,
                // which happens to be the amount of bits a uint uses. int.MinValue would work too.
                appliedPermutation = right | ((newRight >> 32) & (long)uint.MaxValue);
            }
            // shift the left and right half
            var updatedRight = this.GetRightHalf(appliedPermutation);
            appliedPermutation = (appliedPermutation >> 32) & uint.MaxValue;
            appliedPermutation |= updatedRight;
            var result = this.ApplyPermutation(DesConstants.IP1, appliedPermutation);
            return result;
        }

        private long CalculateNewRight(long rightHalf, long key)
        {
            var expandedRightHalf = this.ApplyPermutation(DesConstants.EP, rightHalf);
            var xor = expandedRightHalf ^ key;
            var sBoxesApplied = this.ApplySBoxes(xor);
            var pBoxApplied = this.ApplyPermutation(DesConstants.PBox, sBoxesApplied);
            return pBoxApplied;
        }
        #endregion

        public List<KeyValuePair<string, string>> GetAdditionalInfo()
        {
            var additionalInfo = new List<KeyValuePair<string, string>>();

            additionalInfo.Add(new("Master Key",
                this.MasterKey.ToString("X")));

            foreach (var (key, index) in this.RoundKeys.Select((key, index) => (key, index)))
            {
                additionalInfo.Add(new($"Round Key { index + 1 }",
                    key.ToString("X")));
            }
            return additionalInfo;
        }

        public List<long> Encrypt(string message)
        {
            var result = new List<long>();
            var messageAsLongList = this.StringToLongList(message);
            foreach (var messageAsLong in messageAsLongList)
            {
                var feistelValue = this.Feistel(messageAsLong);
                result.Add(feistelValue);
            }

            return result;
        }

        public string Decrypt(List<long> messageAsLongList)
        {
            var resultAsLongList = new List<long>();

            foreach (var messageAsLong in messageAsLongList)
            {
                var feistelValue = this.Feistel(messageAsLong, true);
                resultAsLongList.Add(feistelValue);
            }

            var resultAsByteList = DesEncryption.LongListToByteList(resultAsLongList);

            return Encoding.UTF8.GetString(resultAsByteList.ToArray());
        }


    }
}
