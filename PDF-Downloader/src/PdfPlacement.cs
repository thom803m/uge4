// Represents a PDF reference with its main and alternative URLs
public class PdfPlacement(string name, string url, string alt_url) // Added "public" to the class, in order to test it 
{
    // The name of the PDF (e.g., used for the filename)
    public string Name { get; set; } = name;

    // The primary URL from which to download the PDF
    public string Url { get; set; } = url;

    // An alternative URL to use if the primary URL fails
    public string Alt_url { get; set; } = alt_url;
}