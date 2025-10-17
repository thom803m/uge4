class Program
{
    static void Main(string[] args)
    {
        // Check that the user has provided at least one argument (the path to the Excel file)
        if (args.Length == 0)
        {
            Console.WriteLine("Need at least a path to the excel sheet");
            return;
        }

        // Default folder for downloaded PDFs
        string folderName = "pdfs";

        // If a second argument is provided, use it as the folder name
        if (args.Length > 1)
            folderName = args[1];

        // Create the folder if it does not already exist
        if (!Directory.Exists($"./{folderName}"))
            Directory.CreateDirectory($"./{folderName}");

        // List to store the PDF links to be downloaded
        List<PdfPlacement> pdfs;

        try
        {
            // Load PDF links from the Excel sheet using XlsxLoader
            pdfs = [.. XlsxLoader.Get_pdf_urls(args[0])];
        }
        catch (Exception e)
        {
            // If the Excel file cannot be opened or read, show an error message and exit
            Console.WriteLine("Could not open " + args[0]);
            Console.WriteLine(e.Message);
            return;
        }

        // List of tasks for asynchronous PDF downloads
        List<Task<PdfResult>> tasks = [];

        // Loop through the first 10 PDFs in the Excel file
        foreach (PdfPlacement pdf in pdfs.Take(10))
        {
            FileDownloader downloader = new(folderName);

            // Skip rows without a valid URL
            if (string.IsNullOrEmpty(pdf.Url))
                continue;

            // Add the download task to the list
            tasks.Add(downloader.Download(pdf));
        }

        // Wait for all downloads to complete
        Task.WaitAll(tasks);
        Console.WriteLine("Download done");

        // Create a metadata Excel file with the results
        if (XlsxMaker.Make_xlsx(tasks.Select(t => t.Result), folderName))
        {
            Console.WriteLine("Metadata sheet created");
        }
        else
        {
            Console.WriteLine("Metadata sheet could not be created");
        }

        // End of program
        return;
    }
}