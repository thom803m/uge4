// Class representing the result of a PDF download
public class PdfResult(string name, string outcome) // Added "public" to the class, in order to test it 
{
    // Name of the PDF file
    public string Name { get; set; } = name;

    // Outcome of the download (e.g., "Downloaded Successfully" or error message)
    public string Outcome { get; set; } = outcome;

    // Override ToString() to provide a readable string representation of the result
    public override string ToString()
    {
        return $"PdfResult({Name}, {Outcome})";
    }
}