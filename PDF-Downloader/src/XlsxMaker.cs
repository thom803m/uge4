using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public partial class XlsxMaker
{
    public static bool Make_xlsx(IEnumerable<PdfResult> _results, string folderName)
    {
        try
        {
            // Create a new Excel (XLSX) file in the target folder.
            // The filename includes the current date for easy tracking.
            using SpreadsheetDocument doc = SpreadsheetDocument.Create(
                $"{folderName}/Metadata{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx",
                DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook);

            // Add a WorkbookPart — this represents the entire Excel file.
            WorkbookPart workbookPart = doc.AddWorkbookPart();
            workbookPart.Workbook = new();

            // Create a WorksheetPart — this represents a single sheet inside the workbook.
            WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

            // Initialize the sheet data structure (rows & cells go here).
            SheetData sheetData = new();

            // Assign the SheetData to the worksheet.
            worksheetPart.Worksheet = new(sheetData);

            // Create a Sheets collection to hold all sheets in the workbook.
            Sheets sheets = workbookPart.Workbook.AppendChild<Sheets>(new());

            // Define a new sheet with ID, name, and link it to the worksheet part.
            Sheet sheet = new()
            {
                Id = doc.WorkbookPart!.GetIdOfPart(worksheetPart),
                SheetId = 1,
                Name = "Metadata"
            };

            // Create the header row (row 1) with column names.
            Row headerRow = new() { RowIndex = DocumentFormat.OpenXml.UInt32Value.FromUInt32((uint)1) };
            headerRow.Append(new Cell()
            {
                CellReference = "A1",
                CellValue = new CellValue("BRnum"),
                DataType = CellValues.String
            });
            headerRow.Append(new Cell()
            {
                CellReference = "B1",
                CellValue = new CellValue("Result"),
                DataType = CellValues.String
            });

            // Add the header row to the SheetData.
            sheetData.Append(headerRow);

            // Convert the incoming IEnumerable<PdfResult> into a list for indexing.
            var results = _results.ToList();

            // Loop through each result and create a new row in the Excel sheet.
            for (int i = 0; i < results.Count; i++)
            {
                Console.WriteLine(results[i]); // Optional debug output

                // Create a new row for each PDF result (starting from row 2)
                Row row = new()
                {
                    RowIndex = DocumentFormat.OpenXml.UInt32Value.FromUInt32((uint)(i + 2))
                };

                // First column: PDF name
                row.Append(new Cell()
                {
                    CellReference = "A" + (i + 2),
                    CellValue = new CellValue(results[i].Name),
                    DataType = CellValues.String
                });

                // Second column: Download result/status
                row.Append(new Cell()
                {
                    CellReference = "B" + (i + 2),
                    CellValue = new CellValue(results[i].Outcome),
                    DataType = CellValues.String
                });

                // Add the row to the sheet
                sheetData.Append(row);
            }

            // Add the sheet to the workbook’s sheet collection
            sheets.Append(sheet);

            // Save workbook and document changes
            workbookPart.Workbook.Save();
            doc.Save();

            // Return true if the process succeeded
            return true;
        }
        catch (Exception)
        {
            // Return false if something failed (e.g., file in use, invalid path, etc.)
            return false;
        }
    }
}
