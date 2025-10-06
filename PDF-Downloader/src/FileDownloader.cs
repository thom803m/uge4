class FileDownloader
{
    private readonly HttpClient client = new();

    public async Task Download(string saveLocation, string url, string alt_url)
    {
        try
        {
            Console.WriteLine(await client.GetStreamAsync(url));
            using Stream downloadStream = await client.GetStreamAsync(url);
            using Stream fileStream = new FileStream(saveLocation, FileMode.Create, FileAccess.Write);

            await downloadStream.CopyToAsync(fileStream);
            await fileStream.FlushAsync();
            fileStream.Close();

            Console.WriteLine(saveLocation + " Downloaded");
        }
        catch (HttpRequestException e)
        {
            if (alt_url != "")
            {
                Console.WriteLine("\nException Caught! Trying alt....");
                await Download(saveLocation, alt_url, "");
            }
            else
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }
    }
}