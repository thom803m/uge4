class FileDownloader(string fileLocation)
{
    private readonly HttpClient client = new();
    private readonly string FileLocation = fileLocation;

    string Pdf_Path(string pdf_name)
    {
        return $"./{FileLocation}/{pdf_name}.pdf";
    }

    public async Task<PdfResult> Download(PdfPlacement pdfPlacement)
    {
        try
        {
            if (!pdfPlacement.Url.StartsWith("http"))
                return new PdfResult(pdfPlacement.Name,"not a valid link");
            Console.WriteLine("Trying {0} with url: {1}%", pdfPlacement.Name, pdfPlacement.Url);
            using Stream downloadStream = await client.GetStreamAsync(pdfPlacement.Url);
            using Stream fileStream = new FileStream(Pdf_Path(pdfPlacement.Name), FileMode.Create, FileAccess.Write);

            await downloadStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileStream.Close();

            return new PdfResult(pdfPlacement.Name, pdfPlacement.Name + " Downloaded Succesfully");
        }
        catch (HttpRequestException e)
        {
            if (pdfPlacement.Alt_url != "")
            {
                Console.WriteLine("\nException Caught! Trying alt....");
                PdfResult result = await Download(new(pdfPlacement.Name, pdfPlacement.Alt_url, ""));
                return new PdfResult(result.Name, result.Outcome + " with alt url");
            }
            else
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return new PdfResult(pdfPlacement.Name, "Failed to access: " + pdfPlacement.Name);
            }
        }
    }
}