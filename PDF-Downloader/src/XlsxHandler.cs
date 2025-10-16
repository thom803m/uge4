using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public partial class XlsxLoader
{
    static string GetCellValue(Cell cell, WorkbookPart workbookPart)
    {
        if (cell is null || cell.CellValue is null)
            return string.Empty;

        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            return workbookPart.SharedStringTablePart.SharedStringTable
                .ElementAt(int.Parse(cell.CellValue.InnerText)).InnerText;
        }
        else
        {
            return cell.CellValue.Text;
        }
    }
    static string Get_colume_reference(Cell cell)
    {
        if (cell == null || cell.CellReference == null)
            return string.Empty;
        return Remove_number().Replace(cell.CellReference, "");
    }
    static public IEnumerable<PdfPlacement> Get_pdf_urls(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new Exception("No path");
        if (!File.Exists(path)) throw new Exception("No such File");

        using SpreadsheetDocument doc = SpreadsheetDocument.Open(path, false);
        WorkbookPart workbookPart = doc.WorkbookPart!;
        WorksheetPart worksheetPart = workbookPart!.WorksheetParts.First();
        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

        IEnumerable<Cell> headerRow = sheetData.Elements<Row>().First().Elements<Cell>();

        List<PdfPlacement> pdfs_to_downloaded = [];

        foreach (Row r in sheetData.Elements<Row>())
        {
            if (r.RowIndex != null && r.RowIndex.Value == 1)
                continue;
            Console.Write("\rCurrent row: " + ((int)r.RowIndex!.Value));
            PdfPlacement pdf_reference = new("","","");
            IEnumerable<Cell> cells = r.Elements<Cell>();
            foreach (Cell cell in cells)
            {
                // Console.WriteLine($"Cell {Get_colume_reference(cell)}: {GetCellValue(cell, workbookPart)}");
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
            pdfs_to_downloaded.Add(pdf_reference);
        }

        return pdfs_to_downloaded;
    }

    [GeneratedRegex("[0-9]")]
    private static partial Regex Remove_number();
}