class Program
{
    static void Main(string[] args)
    {
        // Tjek at brugeren har angivet mindst ét argument (stien til Excel-filen)
        if (args.Length == 0)
        {
            Console.WriteLine("Need at least a path to the excel sheet");
            return;
        }

        // Standardmappe til downloadede PDF’er
        string folderName = "pdfs";

        // Hvis der er angivet et andet argument, bruges det som mappenavn
        if (args.Length > 1)
            folderName = args[1];

        // Opret mappen, hvis den ikke allerede findes
        if (!Directory.Exists($"./{folderName}"))
            Directory.CreateDirectory($"./{folderName}");

        // Liste til at gemme de PDF-links, der skal downloades
        List<PdfPlacement> pdfs;

        try
        {
            // Indlæs PDF-links fra Excel-arket via XlsxLoader
            pdfs = [.. XlsxLoader.Get_pdf_urls(args[0])];
        }
        catch (Exception e)
        {
            // Hvis Excel-filen ikke kan åbnes eller læses, vis fejlbesked og afslut
            Console.WriteLine("Could not open " + args[0]);
            Console.WriteLine(e.Message);
            return;
        }

        // Liste med opgaver (tasks) for asynkrone PDF-downloads
        List<Task<PdfResult>> tasks = [];

        // Loop gennem de første 10 PDF’er i Excel-filen
        foreach (PdfPlacement pdf in pdfs.Take(10))
        {
            FileDownloader downloader = new(folderName);

            // Spring over rækker uden URL
            if (string.IsNullOrEmpty(pdf.Url))
                continue;

            // Tilføj download-opgaven til listen
            tasks.Add(downloader.Download(pdf));
        }

        // Vent på at alle downloads er færdige
        Task.WaitAll(tasks);
        Console.WriteLine("Download done");

        // Opret metadata Excel-fil med resultaterne
        if (XlsxMaker.Make_xlsx(tasks.Select(t => t.Result), folderName))
        {
            Console.WriteLine("Metadata sheet created");
        }
        else
        {
            Console.WriteLine("Metadata sheet could not be created");
        }

        // Afslut programmet
        return;
    }
}