using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public partial class XlsxLoader
{
    // Retrieves the text value from a cell, handling shared strings
    static string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
        if (cell is null || cell.CellValue is null)
            return string.Empty;

        // If the cell is a shared string, look it up in the shared string table
        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            return workbookPart.SharedStringTablePart.SharedStringTable
                .ElementAt(int.Parse(cell.CellValue.InnerText)).InnerText;
        }
        else
        {
            // Otherwise, return the raw text
            return cell.CellValue.Text;
        }
    }

    // Extracts the column letter from a cell reference (e.g., "A1" to "A")
    static string Get_colume_reference(Cell cell)
    {
        if (cell == null || cell.CellReference == null)
            return string.Empty;

        // Remove digits from the reference to leave only letters
        return Remove_number().Replace(cell.CellReference, "");
    }

    // Reads all PDF placements from an Excel file and returns a list
    static public IEnumerable<PdfPlacement> Get_pdf_urls(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new Exception("No path");
        if (!File.Exists(path)) throw new Exception("No such File");

        // Open the Excel document in read-only mode
        using SpreadsheetDocument doc = SpreadsheetDocument.Open(path, false);
        WorkbookPart workbookPart = doc.WorkbookPart!;
        WorksheetPart worksheetPart = workbookPart!.WorksheetParts.First();
        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

        // Get the header row (first row)
        IEnumerable<Cell> headerRow = sheetData.Elements<Row>().First().Elements<Cell>();

        // List to store all PDFs to be downloaded
        List<PdfPlacement> pdfs_to_downloaded = [];

        // Loop through all rows in the sheet
        foreach (Row r in sheetData.Elements<Row>())
        {
            // Skip the header row
            if (r.RowIndex != null && r.RowIndex.Value == 1)
                continue;

            Console.Write("\rCurrent row: " + ((int)r.RowIndex!.Value));

            // Create a new PdfPlacement object for this row
            PdfPlacement pdf_reference = new("", "", "");

            IEnumerable<Cell> cells = r.Elements<Cell>();

            // Loop through each cell in the row
            foreach (Cell cell in cells)
            {
                // Map the cell value to the appropriate property based on column
                switch (Get_colume_reference(cell))
                {
                    case "A":
                        pdf_reference.Name = GetCellValue(cell, workbookPart);
                        break;
                    case "AL":
                        pdf_reference.Url = GetCellValue(cell, workbookPart);
                        break;
                    case "AM":
                        pdf_reference.Alt_url = GetCellValue(cell, workbookPart);
                        break;
                    default:
                        break;
                }
            }

            // Add the PdfPlacement to the list
            pdfs_to_downloaded.Add(pdf_reference);
        }

        // Return the full list of PDF placements
        return pdfs_to_downloaded;
    }

    // Regex to remove numbers from cell references
    [GeneratedRegex("[0-9]")]
    private static partial Regex Remove_number();
}