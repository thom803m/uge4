using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public partial class XlsxMaker
{
    public static bool Make_xlsx(IEnumerable<PdfResult> _results, string folderName)
    {
        try
        {
            using SpreadsheetDocument doc = SpreadsheetDocument.Create($"{folderName}/Metadata{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx", DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);
            WorkbookPart workbookPart = doc.AddWorkbookPart();
            workbookPart.Workbook = new();

            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            SheetData sheetData = new();
            worksheetPart.Worksheet = new(sheetData);

            Sheets sheets = workbookPart.Workbook.AppendChild<Sheets>(new());
            Sheet sheet = new()
            {
                Id = doc.WorkbookPart!.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Metadata"
            };

            Row headerRow = new() { RowIndex = DocumentFormat.OpenXml.UInt32Value.FromUInt32((uint)1) };
            headerRow.Append(new Cell() { CellReference = "A1", CellValue = new CellValue("BRnum"), DataType = CellValues.String });
            headerRow.Append(new Cell() { CellReference = "B1", CellValue = new CellValue("Result"), DataType = CellValues.String });
            sheetData.Append(headerRow);

            var results = _results.ToList();
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine(results[i]);
                Row row = new() { RowIndex = DocumentFormat.OpenXml.UInt32Value.FromUInt32((uint)(i+2)) };
                row.Append(new Cell() { CellReference = "A" + (i+2), CellValue = new CellValue(results[i].Name), DataType = CellValues.String });
                row.Append(new Cell() { CellReference = "B" + (i+2), CellValue = new CellValue(results[i].Outcome), DataType = CellValues.String });
                sheetData.Append(row);
            }

            sheets.Append(sheet);

            workbookPart.Workbook.Save();
            doc.Save();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}