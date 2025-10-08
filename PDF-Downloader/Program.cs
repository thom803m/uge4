class Program
{
    static void Main()
    {
        if (!Directory.Exists("./pdfs"))
            Directory.CreateDirectory("./pdfs");
        // List<(string, string, string)> pdfs = XlsxLoader.Get_pdf_urls("C:/Users/SPAC-23/Documents/Opgaver/uge4/Data/GRI_2017_2020.xlsx");
        // List<Task<(string, string)>> tasks = [];
        // FileDownloader downloader = new();

        // for (int i = 0; i < pdfs.Count; i++)
        // {
        //     if (i > 10)
        //     {
        //         break;
        //     }
        //     if (string.IsNullOrEmpty(pdfs[i].Item2))
        //         continue;
        //     tasks.Add(downloader.Download(pdfs[i].Item1, pdfs[i].Item2, pdfs[i].Item3));
        // }

        // Task.WaitAll(tasks);
        Console.WriteLine("Download done");
        // if (XlsxMaker.Make_xlsx(tasks.Select(t => t.Result)))
        if (XlsxMaker.Make_xlsx([
            ("BR50041", "BR50041 Downloaded"),
            ("BR50042", "BR50042 Downloaded"),
            ("BR50043", "Failed to access: https://www.a2a.eu/en/sustainability/sustainability-reports  with alt url"),
            ("BR50044", "not a valid link"),
            ("BR50045", "BR50045 Downloaded"),
            ("BR50047", "BR50047 Downloaded"),
            ("BR50048", "Failed to access: http://www.aalto.fi/en/about/reports_and_statistics/sustainability/raportit/ with alt url"),
            ("BR50049", "Failed to access: https://www.ag.ch/media/kanton_aargau/bvu/dokumente_2/umwelt__natur___landschaft/naturschutz_1/nachhaltigkeit_1/Bericht_Nachhaltigkeit_Def_klein.pdf"),
            ("BR50050", "BR50050 Downloaded with alt url"),
            ("BR50051", "BR50051 Downloaded")
            ]))
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
