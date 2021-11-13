using System;
using System.Collections.Generic;

namespace SI.Common
{
    /// <summary>
    /// Encryption inteface
    /// </summary>
    public interface IEncryption
    {
        /// <summary>
        /// Get additional encryption info
        /// </summary>
        /// <returns>KeyValue pair representing the additional info</returns>
        List<KeyValuePair<string, string>> GetAdditionalInfo();

        /// <summary>
        /// Encrypts a message
        /// </summary>
        /// <param name="message">Message to encrypt</param>
        /// <returns>Encrypted message</returns>
        string Encrypt(string message);

        /// <summary>
        /// Decrypts an encrypted message
        /// </summary>
        /// <param name="ecryptedMessage">Encrypted message to decrypt</param>
        /// <returns>Decrypted message</returns>
        string Decrypt(string ecryptedMessage);
    }
}
