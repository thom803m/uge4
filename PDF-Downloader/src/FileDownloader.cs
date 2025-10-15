public class FileDownloader
{
    private readonly HttpClient client;
    private readonly string FileLocation;

    // Standard constructor til appen
    public FileDownloader(string fileLocation)
    {
        FileLocation = fileLocation;
        client = new HttpClient();
    }

    // Ny constructor til tests med HttpClient-injektion
    public FileDownloader(string fileLocation, HttpClient httpClient)
    {
        FileLocation = fileLocation;
        client = httpClient;
    }

    private string Pdf_Path(string pdf_name)
    {
        return $"{FileLocation}/{pdf_name}.pdf"; // Har fjernet "./" fordi FileLocation burde være et fuldt path til når der skal testes.
    }

    public async Task<PdfResult> Download(PdfPlacement pdfPlacement)
    {
        try
        {
            if (!pdfPlacement.Url.StartsWith("http"))
                return new PdfResult(pdfPlacement.Name, "not a valid link");

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
            if (!string.IsNullOrEmpty(pdfPlacement.Alt_url))
            {
                Console.WriteLine("\nException Caught! Trying alt....");
                PdfResult result = await Download(new PdfPlacement(pdfPlacement.Name, pdfPlacement.Alt_url, ""));
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
