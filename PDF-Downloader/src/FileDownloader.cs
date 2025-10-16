public class FileDownloader
{
    // HttpClient used to perform HTTP requests for downloading PDFs
    private readonly HttpClient client;

    // Folder where downloaded PDFs will be stored
    private readonly string FileLocation;

    // Standard constructor for the application
    public FileDownloader(string fileLocation)
    {
        FileLocation = fileLocation;
        client = new HttpClient(); // Use default HttpClient
    }

    // Alternative constructor for unit tests, allows injecting a custom HttpClient
    public FileDownloader(string fileLocation, HttpClient httpClient)
    {
        FileLocation = fileLocation;
        client = httpClient; // Use injected HttpClient
    }

    // Helper method to generate the full file path for a PDF
    private string Pdf_Path(string pdf_name)
    {
        return $"{FileLocation}/{pdf_name}.pdf"; // "./" removed because FileLocation is expected to be a full path during tests
    }

    // Main method to download a PDF asynchronously
    public async Task<PdfResult> Download(PdfPlacement pdfPlacement)
    {
        try
        {
            // Check if the URL is valid
            if (!pdfPlacement.Url.StartsWith("http"))
                return new PdfResult(pdfPlacement.Name, "not a valid link");

            Console.WriteLine("Trying {0} with url: {1}%", pdfPlacement.Name, pdfPlacement.Url);

            // Download the PDF as a stream
            using Stream downloadStream = await client.GetStreamAsync(pdfPlacement.Url);

            // Open a file stream to write the PDF to disk
            using Stream fileStream = new FileStream(Pdf_Path(pdfPlacement.Name), FileMode.Create, FileAccess.Write);

            // Copy the downloaded content to the file
            await downloadStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileStream.Close();

            // Return success result
            return new PdfResult(pdfPlacement.Name, pdfPlacement.Name + " Downloaded Succesfully");
        }
        catch (HttpRequestException e)
        {
            // If an alternative URL exists, try downloading from it
            if (!string.IsNullOrEmpty(pdfPlacement.Alt_url))
            {
                Console.WriteLine("\nException Caught! Trying alt....");
                PdfResult result = await Download(new PdfPlacement(pdfPlacement.Name, pdfPlacement.Alt_url, ""));
                return new PdfResult(result.Name, result.Outcome + " with alt url");
            }
            else
            {
                // No alternative URL, return failure with error message
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return new PdfResult(pdfPlacement.Name, "Failed to access: " + pdfPlacement.Name);
            }
        }
    }
}