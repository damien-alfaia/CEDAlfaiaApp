using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.DAL.Parametrage;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CEDAlfaiaAppV2.Code
{
    public static class Impression
    {
        public static Parametrage Parametrage = null;
        public static byte[] JoinArrays(List<byte[]> pdfByteContent)
        {

            using (var ms = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    using (var copy = new PdfSmartCopy(doc, ms))
                    {
                        doc.Open();

                        //Loop through each byte array
                        foreach (var p in pdfByteContent)
                        {

                            //Create a PdfReader bound to that byte array
                            using (var reader = new PdfReader(p))
                            {

                                //Add the entire document instead of page-by-page
                                copy.AddDocument(reader);
                            }
                        }

                        doc.Close();
                    }
                }

                //Return just before disposing
                return ms.ToArray();
            }
        }
    }
}