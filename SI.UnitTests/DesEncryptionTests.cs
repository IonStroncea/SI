using NUnit.Framework;
using SI.DESEncryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI.UnitTests
{
    [TestFixture]
    public class DesEncryptionTests
    {
        DesEncryption des;

        [SetUp]
        public void SetUp()
        {
            this.des = DesEncryption.Get();
        }

        [TestCase("Hello, World!")]
        [TestCase("Hello, World with numbers 5784978454367236743q94508745789!")]
        [TestCase("Let's go a little bit crazy!")]
        [TestCase("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.")]
        public void TestStrings(string textToEcrypt)
        {
            var encrypted = this.des.Encrypt(textToEcrypt);

            var result = this.des.Decrypt(encrypted).Replace("\0", "");

            Assert.IsTrue(result == textToEcrypt);
        }
    }
}
