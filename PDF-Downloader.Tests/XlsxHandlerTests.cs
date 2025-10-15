using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDF_Downloader.Tests
{
    public class XlsxLoaderTests
    {
        [Fact]
        public void GetCellValue_NullCell_ReturnsEmptyString()
        {
            var value = XlsxLoader.GetCellValue(null, null);
            Assert.Equal(string.Empty, value);
        }
    }
}
