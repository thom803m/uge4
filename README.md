# Test Report – PDF Downloader

## 1. Test Overview

| Test Type | Test Class | Test Method | Result | Comment |
|-----------|------------|------------|--------|---------|
| Unit Test | FileDownloaderTests | Download_InvalidUrl_ReturnsFailedResult | ✅ Passed | Handles invalid URLs correctly |
| Unit Test | FileDownloaderTests | Download_ValidUrl_ReturnsSuccess | ✅ Passed | Downloads PDF correctly; file created (line 38 in FileDownloaderTests) |
| Unit Test | FileDownloaderTests | Download_PrimaryUrlFails_UsesAltUrl_ReturnsSuccess | ✅ Passed | Uses alternative URL if primary fails (line 67) |
| Unit Test | XlsxLoaderTests | Get_pdf_urls_ReturnsCorrectPdfPlacements | ✅ Passed | Reads PDF info correctly from Excel (line 54 in XlsxLoader) |
| Unit Test | XlsxMakerTests | Make_xlsx_WithResults_CreatesFile | ✅ Passed | Metadata XLSX is created correctly (line 22 in XlsxMakerTests) |
| Integration Test | IntegrationTests | FullFlow_ExcelToMetadata_CreatesFilesSuccessfully | ✅ Passed | Tests the full flow: Excel → download → metadata |

---

## 2. Test Environment

- **IDE:** Visual Studio 2022  
- **.NET Version:** 9.0  
- **Packages:**  
  - Moq (unit tests, mocking HttpClient)  
  - Xunit (test framework)  
  - DocumentFormat.OpenXml (Excel/XLSX)  

---

## 3. Test Strategy

- **Unit Tests:**  
  - Isolate core methods using mocks (HttpClient)  
  - Test both positive and negative scenarios  
  - Focus on FileDownloader, XlsxLoader, and XlsxMaker  

- **Integration Tests:**  
  - Test the full flow without mocks  
  - Test interaction between Excel → PDF → metadata  
  - Ensure files are created and metadata is saved correctly  

---

## 4. Errors and Observations

- **NullReferenceException in XlsxLoader**  
  - Cause: Missing header in test Excel file  
  - Fixed by adding the header and correct row content  

- **Metadata XLSX test failed if folder did not exist**  
  - Fixed using `Directory.CreateDirectory(folder)` in the test  

- **FileDownloader unit test required correct folder path**  
  - Fixed using `Path.Combine(Path.GetTempPath(), "folder")`  

- Lines in code where tests revealed issues:  
  - `FileDownloader.Download` lines 35–38 (folder existence & file creation)  
  - `XlsxLoader.Get_pdf_urls` lines 46–54 (null-check on cells)  
  - `XlsxMaker.Make_xlsx` lines 22–40 (ensure SheetData and rows exist)  

---

## 5. Code Quality Evaluation

- **Naming:**  
  - Classes and methods follow consistent PascalCase/camelCase conventions  
  - Names are descriptive: `FileDownloader`, `XlsxLoader`, `Download_ValidUrl_ReturnsSuccess`  

- **Structure and Maintainability:**  
  - Methods are relatively short and focused  
  - Partial classes (`XlsxLoader`, `XlsxMaker`) provide flexibility but should be well-documented to indicate which methods belong where  

- **Error Handling:**  
  - Download handles invalid URLs and HttpRequestExceptions with fallback to alternative URL  
  - Logging is missing in production – recommended for future debugging  

- **Comments:**  
  - Comments exist in complex parts, but some methods could explain why certain columns are read (e.g., AL, AM in Excel)  

---

## 6. Test Coverage and Limitations

- **Covered scenarios:**  
  - Valid and invalid URLs  
  - Primary URL fails → fallback to alt URL  
  - Excel data read correctly  
  - Metadata XLSX created  

- **Limitations:**  
  - Network tests with real PDFs are not run in unit tests → mocks used  
  - Maximum of 10 PDFs per integration test to avoid network issues  
  - Not all edge cases for Excel formats tested (e.g., empty cells without header)  

---

## 7. Improvement Suggestions (#TODO)

- Robust file handling: check for existing files, handle write errors gracefully  
- Add logging to Download method for better debugging  
- Dedicated “TestData” folder with small PDFs for integration testing  
- CI/CD pipeline to automatically run all tests on commit  

---

## 8. Test Data Setup

The integration tests require the Excel file **GRI_2017_2020.xlsx**.  

1. Create a `data` folder in the root of the project if it does not exist:  

2. Place the Excel file inside the `data` folder.  

3. Add the following link in Markdown for easy access:

[Download GRI_2017_2020.xlsx](./data/GRI_2017_2020.xlsx)

> 💡 Note: On GitHub, clicking this link will allow you to download the file to your local machine.

---

## 9. Conclusion

- All critical functionality is covered by tests  
- Full flow Excel → PDF → Metadata works correctly  
- Code is now testable, modular, and robust against URL and file system errors  
- The report documents errors, fixes, code quality, and suggested improvements
