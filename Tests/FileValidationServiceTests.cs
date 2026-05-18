using AgriConnectPrototype.Services;

namespace AgriConnectPrototype.Tests
{
    // Tests uploaded file validation
    // Only PDF agreement files should be accepted

    public class FileValidationServiceTests
    {
        [Fact]

        // Verify that restricted file types are rejected
        public void ValidatePdf_ShouldThrowError_WhenFileIsExe()
        {
            // Create service object
            var service = new FileValidationService();

            // Test invalid file type
            Assert.Throws<Exception>(
                () => service.ValidatePdf("setup.exe"));
        }

        [Fact]

        // Verify that PDF files are accepted
        public void ValidatePdf_ShouldNotThrowError_WhenFileIsPdf()
        {
            // Create service object
            var service = new FileValidationService();

            // Test valid PDF file
            var exception =
                Record.Exception(
                () => service.ValidatePdf("agreement.pdf"));

            // No exception should occur
            Assert.Null(exception);
        }
    }
}