class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0) { Console.WriteLine("Need at least a path to the excel sheet"); return; }
        string folderName = "pdfs";
        if (args.Length > 1)
            folderName = args[1];
        if (!Directory.Exists($"./{folderName}"))
            Directory.CreateDirectory($"./{folderName}");

        List<PdfPlacement> pdfs;
        try { pdfs = [.. XlsxLoader.Get_pdf_urls(args[0])]; }
        catch (Exception e)
        {
            Console.WriteLine("Could not open " + args[0]);
            Console.WriteLine(e.Message);
            return;
        }

        List<Task<PdfResult>> tasks = [];
        foreach (PdfPlacement pdf in pdfs.Take(10))
        {
            FileDownloader downloader = new(folderName);
            if (string.IsNullOrEmpty(pdf.Url))
                continue;
            tasks.Add(downloader.Download(pdf));
        }

        Task.WaitAll(tasks);
        Console.WriteLine("Download done");
        if (XlsxMaker.Make_xlsx(tasks.Select(t => t.Result), folderName))
        {
            Console.WriteLine("Metadata sheet created");
        }
        else
        {
            Console.WriteLine("Metadata sheet could not be created");
        }
        return;
    }
}
