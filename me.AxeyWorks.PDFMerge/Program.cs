using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace me.AxeyWorks.PDFMerge
{
    static class Program
    {
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Our merged file name
            string finalFile = "Merged PDF.pdf";

            // Clean up after people
            File.Delete(finalFile);

            string path = Directory.GetCurrentDirectory();
            // Load all pdf files
            string[] files = Directory.GetFiles(path, "*.PDF");
            List<byte[]> filesByte = new List<byte[]>();
            foreach (string file in files)
            {
                filesByte.Add(File.ReadAllBytes(file));
            }
            // Call pdf merger
            File.WriteAllBytes(finalFile, MergeFiles(filesByte));
        }

        public static byte[] MergeFiles(List<byte[]> sourceFiles)
        {
            Document document = new Document();
            using (MemoryStream ms = new MemoryStream())
            {
                PdfCopy copy = new PdfCopy(document, ms);
                document.Open();
                int documentPageCounter = 0;

                // Iterate through all pdf documents
                for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                {
                    // Create pdf reader
                    PdfReader reader = new PdfReader(sourceFiles[fileCounter]);
                    int numberOfPages = reader.NumberOfPages;

                    // Iterate through all pages
                    for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                    {
                        PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);

                        //***************************************
                        // Uncomment and alter for watermarking.
                        //***************************************
                        //
                        //documentPageCounter++;
                        //PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);
                        //
                        //// Write header
                        //ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                        //    new Phrase("PDF Merged with me.AxeyWorks PDFMerge"), importedPage.Width / 2, importedPage.Height - 30,
                        //    importedPage.Width < importedPage.Height ? 0 : 1);
                        //
                        //// Write footer
                        //ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                        //    new Phrase($"Page {documentPageCounter}"), importedPage.Width / 2, 30,
                        //    importedPage.Width < importedPage.Height ? 0 : 1);
                        //
                        //pageStamp.AlterContents();

                        copy.AddPage(importedPage);
                    }

                    copy.FreeReader(reader);
                    reader.Close();
                }

                document.Close();
                return ms.GetBuffer();
            }
        }
    }
}