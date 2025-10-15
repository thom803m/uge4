public class PdfResult(string name, string outcome) // Puttet "public" foran klassen, så jeg kan teste den
{
    public string Name { get; set; } = name;
    public string Outcome { get; set; } = outcome;

    public override string ToString()
    {
        return $"PdfResult({Name}, {Outcome})";
    }
}