using Moq;
using Moq.Protected;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using System.IO;

namespace PDF_Downloader.Tests.UnitTests
{
    public class FileDownloaderTests
    {
        [Fact]
        public async Task Download_InvalidUrl_ReturnsFailedResult()
        {
            // Arrange: create a FileDownloader instance with a test folder
            var downloader = new FileDownloader("testFolder");

            // Create a PDF reference with an invalid URL
            var pdf = new PdfPlacement("TestPdf", "invalid_url", "");

            // Act: attempt to download
            var result = await downloader.Download(pdf);

            // Assert: check that the outcome contains the "not a valid link" message
            Assert.Contains("not a valid link", result.Outcome);
        }

        [Fact]
        public async Task Download_PrimaryUrlFails_UsesAltUrl_ReturnsSuccess()
        {
            // Arrange: create a temporary folder for the test
            string folder = Path.Combine(Path.GetTempPath(), "testFolderAltUrl");
            Directory.CreateDirectory(folder);

            try
            {
                // Mock HttpMessageHandler to simulate HTTP requests
                var handlerMock = new Mock<HttpMessageHandler>();

                // Simulate primary URL failure
                handlerMock
                    .Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString() == "http://primary.url/test.pdf"),
                        ItExpr.IsAny<CancellationToken>()
                    )
                    .ThrowsAsync(new HttpRequestException("Simulated failure for primary URL"));

                // Simulate successful response for alternative URL
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

                // Inject the mocked HttpClient into FileDownloader
                var httpClient = new HttpClient(handlerMock.Object);
                var downloader = new FileDownloader(folder, httpClient);

                // Create a PDF reference with primary and alternative URLs
                var pdf = new PdfPlacement("TestPdfAlt", "http://primary.url/test.pdf", "http://alt.url/test.pdf");

                // Act: attempt to download
                var result = await downloader.Download(pdf);

                // Assert: confirm that alternative URL was used
                Assert.Contains("with alt url", result.Outcome);

                // Assert: check that the PDF file was created
                Assert.True(File.Exists(Path.Combine(folder, "TestPdfAlt.pdf")));
            }
            finally
            {
                // Cleanup: delete the downloaded file
                string filePath = Path.Combine(folder, "TestPdfAlt.pdf");
                if (File.Exists(filePath))
                    File.Delete(filePath);

                // Cleanup: remove the folder if empty
                if (Directory.Exists(folder) && Directory.GetFileSystemEntries(folder).Length == 0)
                    Directory.Delete(folder);
            }
        }

        [Fact]
        public async Task Download_ValidUrl_ReturnsSuccess()
        {
            // Arrange: create a temporary folder for the test
            string folder = Path.Combine(Path.GetTempPath(), "testFolder");
            Directory.CreateDirectory(folder);

            try
            {
                // Mock HttpMessageHandler to simulate HTTP request
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

                // Inject mocked HttpClient into FileDownloader
                var httpClient = new HttpClient(handlerMock.Object);
                var downloader = new FileDownloader(folder, httpClient);

                // Create a PDF reference with a valid URL
                var pdf = new PdfPlacement("TestPdf", "http://valid.url/test.pdf", "");

                // Act: download the PDF
                var result = await downloader.Download(pdf);

                // Assert: verify that the download succeeded
                Assert.Contains("Downloaded Succesfully", result.Outcome);

                // Assert: check that the PDF file exists
                Assert.True(File.Exists(Path.Combine(folder, "TestPdf.pdf")));
            }
            finally
            {
                // Cleanup: delete the downloaded file
                string filePath = Path.Combine(folder, "TestPdf.pdf");
                if (File.Exists(filePath))
                    File.Delete(filePath);

                // Cleanup: remove the folder if empty
                if (Directory.Exists(folder) && Directory.GetFileSystemEntries(folder).Length == 0)
                    Directory.Delete(folder);
            }
        }
    }
}