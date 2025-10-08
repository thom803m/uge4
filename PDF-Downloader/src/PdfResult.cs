class PdfResult(string name, string outcome)
{
    public string Name { get; set; } = name;
    public string Outcome { get; set; } = outcome;

    public override string ToString()
    {
        return $"PdfResult({Name}, {Outcome})";
    }
}