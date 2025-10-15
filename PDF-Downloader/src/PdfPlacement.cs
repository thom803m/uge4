public class PdfPlacement(string name, string url, string alt_url) // Puttet "public" foran klassen, så jeg kan teste den
{
    public string Name { get; set; } = name;
    public string Url { get; set; } = url;
    public string Alt_url { get; set; } = alt_url;
}