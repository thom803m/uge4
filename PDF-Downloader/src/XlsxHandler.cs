using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

class XlsxHandler
{
    public List<(string, string, string)> Get_pdf_urls(string path)
    {
        if (string.IsNullOrEmpty(path)) throw new Exception("No path");
        if (!File.Exists(path)) throw new Exception("No such File");

        using SpreadsheetDocument doc = SpreadsheetDocument.Open(path, false);
        WorkbookPart workbookPart = doc.WorkbookPart;
        WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
        SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

        IEnumerable<Cell> headerRow = sheetData.Elements<Row>().First().Elements<Cell>();

        List<(string, string, string)> pdfs_to_downloaded = [];
        (int, int, int) headerValues = (0,0,0);
        for (Cell cell in headerRow)
        {
            if(cell.InnerText == "BRnum") headerValues.Item1
        }

        foreach (Row r in sheetData.Elements<Row>())
            {
                IEnumerable<Cell> cells = r.Elements<Cell>();
                foreach (Cell cell in cells)
                {
                    cell.
                }
            }

        
    }
}