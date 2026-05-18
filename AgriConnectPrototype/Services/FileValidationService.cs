namespace AgriConnectPrototype.Services
{
    // Validates uploaded agreement files.
    // The Farmers system only allows PDF signed agreements.

    public class FileValidationService
    {
        // Checks whether the uploaded file is a PDF
        public void ValidatePdf(string fileName)
        {
            // Get file extension
            var extension = Path.GetExtension(fileName).ToLower();

            // Reject files that are not PDF
            if (extension != ".pdf")
            {
                throw new Exception("Only PDF files are allowed.");
            }
        }
    }
}