using Xunit;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace PDF_Downloader.Tests.IntegrationTests
{
    public class IntegrationTests
    {
        [Fact]
        public async Task FullFlow_ExcelToMetadata_CreatesFilesSuccessfully()
        {
            // Create a temporary folder for the integration test
            string folder = Path.Combine(Path.GetTempPath(), "IntegrationTest");
            Directory.CreateDirectory(folder);

            try
            {
                // Create a test Excel file with one PDF entry
                string excelPath = Path.Combine(folder, "test.xlsx");
                var pdfUrl = "https://www.w3.org/WAI/ER/tests/xhtml/testfiles/resources/pdf/dummy.pdf";

                TestHelpers.CreateTestExcel(excelPath, new[]
                {
                    new PdfPlacement("TestPdf", pdfUrl, "")
                });

                // Load the list of PDFs from the Excel file using XlsxLoader
                var pdfs = XlsxLoader.Get_pdf_urls(excelPath);

                // Download PDFs asynchronously
                var tasks = pdfs.Select(pdf =>
                {
                    var downloader = new FileDownloader(folder);
                    return downloader.Download(pdf);
                }).ToArray();

                await Task.WhenAll(tasks);

                // Create metadata Excel file from downloaded PDF results
                bool metadataCreated = XlsxMaker.Make_xlsx(tasks.Select(t => t.Result), folder);

                // Assertions: ensure files are created correctly
                Assert.True(metadataCreated);
                Assert.True(File.Exists(Path.Combine(folder, "TestPdf.pdf")));
                Assert.True(Directory.GetFiles(folder).Any(f => f.EndsWith(".xlsx")));
            }
            finally
            {
                // Cleanup: delete all files and the folder after test
                foreach (var f in Directory.GetFiles(folder))
                    File.Delete(f);

                if (Directory.Exists(folder))
                    Directory.Delete(folder);
            }
        }
    }

    // Helper class to create test Excel files
    public static class TestHelpers
    {
        public static void CreateTestExcel(string path, PdfPlacement[] pdfs)
        {
            using var doc = SpreadsheetDocument.Create(path, SpreadsheetDocumentType.Workbook);
            var workbookPart = doc.AddWorkbookPart();
            workbookPart.Workbook = new Workbook();
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            var sheetData = new SheetData();

            // Create header row
            var headerRow = new Row() { RowIndex = 1 };
            headerRow.Append(
                new Cell() { CellReference = "A1", CellValue = new CellValue("Name"), DataType = CellValues.String },
                new Cell() { CellReference = "AL1", CellValue = new CellValue("Url"), DataType = CellValues.String },
                new Cell() { CellReference = "AM1", CellValue = new CellValue("AltUrl"), DataType = CellValues.String }
            );
            sheetData.Append(headerRow);

            // Add data rows for each PDF
            int rowIndex = 2;
            foreach (var pdf in pdfs)
            {
                var row = new Row() { RowIndex = (uint)rowIndex };
                row.Append(
                    new Cell() { CellReference = $"A{rowIndex}", CellValue = new CellValue(pdf.Name), DataType = CellValues.String },
                    new Cell() { CellReference = $"AL{rowIndex}", CellValue = new CellValue(pdf.Url), DataType = CellValues.String },
                    new Cell() { CellReference = $"AM{rowIndex}", CellValue = new CellValue(pdf.Alt_url), DataType = CellValues.String }
                );
                sheetData.Append(row);
                rowIndex++;
            }

            // Assign sheet data to the worksheet
            worksheetPart.Worksheet = new Worksheet(sheetData);

            // Add the worksheet to the workbook
            var sheets = workbookPart.Workbook.AppendChild(new Sheets());
            sheets.Append(new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Sheet1"
            });

            // Save the workbook
            workbookPart.Workbook.Save();
        }
    }
}