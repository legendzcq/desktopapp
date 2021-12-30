
using DownloadClass.Toolkit.Models;
using DownloadClass.Toolkit.Services;

using Microsoft.Extensions.Logging.Abstractions;

using Xunit;

namespace DownloadClass.Toolkit.Tests
{
    public class DecryptorTests
    {
        [Fact]
        public void TryDecrypteXml_CorrectEncrypted_ReturnsTrue()
        {
            var decrypter = new Decryptor(NullLogger<Decryptor>.Instance);

            bool result = decrypter.TryDecrypteXml("Assets/acc131541a/config.xml", out Courseware courseware);

            Assert.True(result);
            Assert.Equal(37075, courseware.CoursewareId);
        }

        [Fact]
        public void DecrypteXml_CorrectEncrypted_ReturnsCourseware()
        {

            var decrypter = new Decryptor(NullLogger<Decryptor>.Instance);

            Courseware result = decrypter.DecrypteXml("Assets/acc131541a/config.xml");

            Assert.Equal(37075, result.CoursewareId);
        }
    }
}
