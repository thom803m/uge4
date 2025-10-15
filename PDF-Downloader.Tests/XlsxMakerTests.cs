using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Downloader.Tests
{
    public class XlsxMakerTests
    {
        [Fact]
        public void Make_xlsx_WithResults_CreatesFile()
        {
            string folder = Path.Combine(Path.GetTempPath(), "testFolder");
            Directory.CreateDirectory(folder);

            var results = new List<PdfResult>
            {
                new PdfResult("Test1", "Success"),
                new PdfResult("Test2", "Failed")
            };

            bool success = XlsxMaker.Make_xlsx(results, folder);

            Assert.True(success);
            Assert.True(File.Exists(Path.Combine(folder, "Metadata" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx")));
        }

    }
}
