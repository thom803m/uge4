using Moq;
using Moq.Protected;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.IO;

namespace PDF_Downloader.Tests
{
    public class FileDownloaderTests
    {
        [Fact]
        public async Task Download_InvalidUrl_ReturnsFailedResult()
        {
            // Arrange
            var downloader = new FileDownloader("testFolder");
            var pdf = new PdfPlacement("TestPdf", "invalid_url", "");

            // Act
            var result = await downloader.Download(pdf);

            // Assert
            Assert.Contains("not a valid link", result.Outcome);
        }

        [Fact]
        public async Task Download_PrimaryUrlFails_UsesAltUrl_ReturnsSuccess()
        {
            string folder = Path.Combine(Path.GetTempPath(), "testFolderAltUrl");
            Directory.CreateDirectory(folder);

            try
            {
                var handlerMock = new Mock<HttpMessageHandler>();

                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "http://primary.url/test.pdf"),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ThrowsAsync(new HttpRequestException("Simulated failure for primary URL"));

                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "http://alt.url/test.pdf"),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("Fake PDF content from alt URL")
                    });

                var httpClient = new HttpClient(handlerMock.Object);
                var downloader = new FileDownloader(folder, httpClient);
                var pdf = new PdfPlacement("TestPdfAlt", "http://primary.url/test.pdf", "http://alt.url/test.pdf");

                var result = await downloader.Download(pdf);

                Assert.Contains("with alt url", result.Outcome);
                Assert.True(File.Exists(Path.Combine(folder, "TestPdfAlt.pdf")));
            }
            finally
            {
                string filePath = Path.Combine(folder, "TestPdfAlt.pdf");
                if (File.Exists(filePath))
                    File.Delete(filePath);

                if (Directory.Exists(folder) && Directory.GetFileSystemEntries(folder).Length == 0)
                    Directory.Delete(folder);
            }
        }

        [Fact]
        public async Task Download_ValidUrl_ReturnsSuccess()
        {
            string folder = Path.Combine(Path.GetTempPath(), "testFolder");
            Directory.CreateDirectory(folder);

            try
            {
                var handlerMock = new Mock<HttpMessageHandler>();
                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ReturnsAsync(new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new StringContent("Fake PDF content")
                    });

                var httpClient = new HttpClient(handlerMock.Object);
                var downloader = new FileDownloader(folder, httpClient);
                var pdf = new PdfPlacement("TestPdf", "http://valid.url/test.pdf", "");

                var result = await downloader.Download(pdf);

                Assert.Contains("Downloaded Succesfully", result.Outcome);
                Assert.True(File.Exists(Path.Combine(folder, "TestPdf.pdf")));
            }
            finally
            {
                string filePath = Path.Combine(folder, "TestPdf.pdf");
                if (File.Exists(filePath))
                    File.Delete(filePath);

                if (Directory.Exists(folder) && Directory.GetFileSystemEntries(folder).Length == 0)
                    Directory.Delete(folder);
            }
        }
    }
}