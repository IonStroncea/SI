using NUnit.Framework;
using SI.RSAEncryption;

namespace SI.UnitTests
{
    [TestFixture]
    public class RsaEncryptionTests
    {
        RsaEncryption rsa;

        [SetUp]
        public void SetUp()
        {
            this.rsa = RsaEncryption.Get();
        }

        [TestCase("Hello, World!")]
        [TestCase("Hello, World with numbers 5784978454367236743q94508745789!")]
        [TestCase("Let's go a little bit crazy!")]
        [TestCase("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.")]
        public void TestStrings(string textToEcrypt)
        {
            var encryptedText = this.rsa.Encrypt(textToEcrypt);

            var result = this.rsa.Decrypt(encryptedText);

            Assert.IsTrue(result == textToEcrypt);
        }
    }
}