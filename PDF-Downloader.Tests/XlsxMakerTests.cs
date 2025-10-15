using Xunit;
using System.IO;
using System.Collections.Generic;

namespace PDF_Downloader.Tests
{
    public class XlsxMakerTests
    {
        [Fact]
        public void Make_xlsx_WithResults_CreatesFile()
        {
            string folder = Path.Combine(Path.GetTempPath(), "testXlsx");
            Directory.CreateDirectory(folder);

            var results = new List<PdfResult>
        {
            new PdfResult("Test1", "Success"),
            new PdfResult("Test2", "Failed")
        };

            bool success = XlsxMaker.Make_xlsx(results, folder);

            Assert.True(success);

            string[] files = Directory.GetFiles(folder, "*.xlsx");
            Assert.Single(files);

            Directory.Delete(folder, true);
        }
    }
};