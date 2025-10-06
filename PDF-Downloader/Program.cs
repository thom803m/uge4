class Program
{
    static string Pdf_Path(string pdf_name)
    {
        return "./pdfs/" + pdf_name + ".pdf";
    }
    static void Main()
    {
        if (!Directory.Exists("./pdfs"))
            Directory.CreateDirectory("./pdfs");
        List<(string, string, string)> pdfs = [
            (Pdf_Path("BR50045"), "http://www.hkexnews.hk/listedco/listconews/SEHK/2017/0512/LTN20170512165.pdf", ""),
            (Pdf_Path("BR50047"), "http://ebooks.exakta.se/aak/2017/hallbarhetsrapport_2016_2017_en/pubData/source/aak_sustainability_report_2016_2017_ebook.pdf", ""),
            (Pdf_Path("BR50048"), "http://www.aalto.fi/en/midcom-serveattachmentguid-1e725b2da56a9aa25b211e7b2d767a02c5c752a752a/sustainability_at_aalto_university_2016_fi.pdf", "http://www.aalto.fi/en/about/reports_and_statistics/sustainability/raportit/"),
            (Pdf_Path("BR50049"), "https://www.ag.ch/media/kanton_aargau/bvu/dokumente_2/umwelt__natur___landschaft/naturschutz_1/nachhaltigkeit_1/Bericht_Nachhaltigkeit_Def_klein.pdf", ""),
            (Pdf_Path("BR50050"), "https://www.akb.ch/documents/30573/92310/nachhaltigkeitsbericht-2016.pdf", "https://www.akb.ch/die-akb/kommunikation-medien/geschaeftsberichte"),
            (Pdf_Path("BR50051"), "http://www.eugesta.lt/assets/SOCIALINES-ATSAKOMYBES-ATASKAITA-2016.pdf", "")           
            ];
        List<Task> tasks = [];
        FileDownloader downloader = new();

        for (int i = 0; i < pdfs.Count; i++)
        {
            if (i > 10)
            {
                break;
            }
            if (String.IsNullOrEmpty(pdfs[i].Item2))
                continue;
            tasks.Add(downloader.Download(pdfs[i].Item1, pdfs[i].Item2, pdfs[i].Item3));
        }

        Task.WaitAll(tasks);
        Console.WriteLine("Download done");
    }
}
