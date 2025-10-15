using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.IO;
using System.Linq;
using Xunit;

namespace PDF_Downloader.Tests
{
    public class XlsxLoaderTests
    {
        [Fact]
        public void Get_pdf_urls_ReturnsCorrectPdfPlacements()
        {
            string folder = Path.Combine(Path.GetTempPath(), "XlsxLoaderTest");
            Directory.CreateDirectory(folder);
            string filePath = Path.Combine(folder, "test.xlsx");

            using (var doc = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = doc.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                SheetData sheetData = new SheetData();

                Row headerRow = new Row() { RowIndex = 1 };
                headerRow.Append(
                    new Cell() { CellReference = "A1", CellValue = new CellValue("Name"), DataType = CellValues.String },
                    new Cell() { CellReference = "B1", CellValue = new CellValue("Dummy"), DataType = CellValues.String },
                    new Cell() { CellReference = "AL1", CellValue = new CellValue("Url"), DataType = CellValues.String },
                    new Cell() { CellReference = "AM1", CellValue = new CellValue("AltUrl"), DataType = CellValues.String }
                );
                sheetData.Append(headerRow);

                Row dataRow = new Row() { RowIndex = 2 };
                dataRow.Append(
                    new Cell() { CellReference = "A2", CellValue = new CellValue("TestPdf"), DataType = CellValues.String },
                    new Cell() { CellReference = "B2", CellValue = new CellValue(""), DataType = CellValues.String },
                    new Cell() { CellReference = "AL2", CellValue = new CellValue("http://valid.url/test.pdf"), DataType = CellValues.String },
                    new Cell() { CellReference = "AM2", CellValue = new CellValue("http://alt.url/test.pdf"), DataType = CellValues.String }
                );
                sheetData.Append(dataRow);

                worksheetPart.Worksheet = new Worksheet(sheetData);

                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                Sheet sheet = new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "Sheet1"
                };
                sheets.Append(sheet);

                workbookPart.Workbook.Save();
            }

            var pdfs = XlsxLoader.Get_pdf_urls(filePath).ToList();

            Assert.Single(pdfs);
            Assert.Equal("TestPdf", pdfs[0].Name);
            Assert.Equal("http://valid.url/test.pdf", pdfs[0].Url);
            Assert.Equal("http://alt.url/test.pdf", pdfs[0].Alt_url);

            Directory.Delete(folder, true);
        }
    }
}