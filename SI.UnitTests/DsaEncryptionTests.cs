using NUnit.Framework;
using SI.DSAEncryption;

namespace SI.UnitTests
{
    [TestFixture]
    public class DsaEncryptionTests
    {
        DsaEncryption dsa;

        [SetUp]
        public void SetUp()
        {
            this.dsa = DsaEncryption.Get();
        }

        [TestCase("Hello, World!")]
        [TestCase("Hello, World with numbers 5784978454367236743q94508745789!")]
        [TestCase("Let's go a little bit crazy!")]
        [TestCase("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.")]
        public void TestStrings(string message)
        {
            var signature = this.dsa.SignData(message);

            var result = this.dsa.Verify(message, signature);

            Assert.IsTrue(result);
        }

        [TestCase("Hello, World!")]
        [TestCase("Hello, World with numbers 5784978454367236743q94508745789!")]
        [TestCase("Let's go a little bit crazy!")]
        [TestCase("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.")]
        public void TestStrings_Should_Fail_Because_Of_Message_Changing(string message)
        {
            var signature = this.dsa.SignData(message);
            
            var result = this.dsa.Verify(message + "a", signature);

            Assert.IsFalse(result);
        }
    }
}