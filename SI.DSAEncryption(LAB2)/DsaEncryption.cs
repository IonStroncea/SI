using System;
using System.Collections.Generic;

namespace SI.DSAEncryption
{
    public class DsaEncryption
    {

        private DsaEncryption()
        {

        }

        #region GenerationLogic


        /// <summary>
        /// Generate and returns the encryption method
        /// </summary>
        /// <returns>Encryption object of type DSA</returns>
        public static DsaEncryption Get()
        {

            return new DsaEncryption();
        }
        #endregion

        public List<KeyValuePair<string, string>> GetAdditionalInfo()
        {
            throw new NotImplementedException();
        }

        public string Encrypt(string message)
        {
            throw new NotImplementedException();
        }

        public string Decrypt(string ecryptedMessage)
        {
            throw new NotImplementedException();
        }
    }
}
