using Xunit;
using System.IO;
using System.Collections.Generic;

namespace PDF_Downloader.Tests.UnitTests
{
    public class XlsxMakerTests
    {
        [Fact]
        public void Make_xlsx_WithResults_CreatesFile()
        {
            // Create a temporary folder to store the generated XLSX
            string folder = Path.Combine(Path.GetTempPath(), "testXlsx");
            Directory.CreateDirectory(folder);

            // Prepare a list of sample PDF results to write to the XLSX
            var results = new List<PdfResult>
            {
                new PdfResult("Test1", "Success"),  // First PDF result
                new PdfResult("Test2", "Failed")    // Second PDF result
            };

            // Call XlsxMaker to create the XLSX file with the provided results
            bool success = XlsxMaker.Make_xlsx(results, folder);

            // Assert that the XLSX creation returned true (successful)
            Assert.True(success);

            // Verify that exactly one XLSX file exists in the folder
            string[] files = Directory.GetFiles(folder, "*.xlsx");
            Assert.Single(files);

            // Cleanup: Delete all files and the temporary folder
            Directory.Delete(folder, true);
        }
    }
};