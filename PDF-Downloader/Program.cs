class Program
{
    static void Main()
    {
        if (!Directory.Exists("./pdfs"))
            Directory.CreateDirectory("./pdfs");
        List<(string, string, string)> pdfs = XlsxLoader.Get_pdf_urls("C:/Users/SPAC-23/Documents/Opgaver/uge4/Data/GRI_2017_2020.xlsx");
        List<Task<(string, string)>> tasks = [];
        FileDownloader downloader = new();

        for (int i = 0; i < pdfs.Count; i++)
        {
            if (i > 10)
            {
                break;
            }
            if (string.IsNullOrEmpty(pdfs[i].Item2))
                continue;
            tasks.Add(downloader.Download(pdfs[i].Item1, pdfs[i].Item2, pdfs[i].Item3));
        }

        Task.WaitAll(tasks);
        Console.WriteLine("Download done");
        if (XlsxMaker.Make_xlsx(tasks.Select(t => t.Result)))
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
