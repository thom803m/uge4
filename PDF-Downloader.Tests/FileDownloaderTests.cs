using Moq;
using Moq.Protected;
using System.Net;
using System.Threading.Tasks;
using Xunit;


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
        public async Task Download_PrimaryUrlFails_UsesAltUrl()
        {
            var downloader = new FileDownloader("testFolder");
            var pdf = new PdfPlacement("TestPdf", "http://invalid.url", "http://example.com/test.pdf");

            var result = await downloader.Download(pdf);

            Assert.Contains("with alt url", result.Outcome);
        }

        [Fact]
        public async Task Download_ValidUrl_ReturnsSuccess()
        {
            string folder = Path.Combine(Path.GetTempPath(), "testFolder");
            Directory.CreateDirectory(folder);

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
    }
};