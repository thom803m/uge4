class FileDownloader
{
    private readonly HttpClient client = new();

    static string Pdf_Path(string pdf_name)
    {
        return "./pdfs/" + pdf_name + ".pdf";
    }

    public async Task<(string, string)> Download(string saveLocation, string url, string alt_url)
    {
        try
        {
            if (!url.StartsWith("http"))
                return (saveLocation,"not a valid link");
            Console.WriteLine("Trying {0} with url: {1}%", saveLocation, url);
            using Stream downloadStream = await client.GetStreamAsync(url);
            using Stream fileStream = new FileStream(Pdf_Path(saveLocation), FileMode.Create, FileAccess.Write);

            await downloadStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileStream.Close();

            return (saveLocation, saveLocation + " Downloaded Succesfully");
        }
        catch (HttpRequestException e)
        {
            if (alt_url != "")
            {
                Console.WriteLine("\nException Caught! Trying alt....");
                (string, string) result = await Download(saveLocation, alt_url, "");
                return (result.Item1, result.Item2 + " with alt url");
            }
            else
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
                return (saveLocation, "Failed to access: " + saveLocation);
            }
        }
        // catch (Exception e)
        // {
        //         Console.WriteLine("\nBad url Exception Caught!");
        //         Console.WriteLine("pdf :{0}, at url: {1}", saveLocation, url);
        //         Console.WriteLine("Message :{0} ", e.Message);
        // }
    }
}