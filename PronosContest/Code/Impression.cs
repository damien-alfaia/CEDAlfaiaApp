using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp.text.pdf;
using iTextSharp.text;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.DAL.Parametrage;
using System.IO;

namespace CEDAlfaiaApp.Code
{
    public static class Impression
    {
        public static Parametrage Parametrage = null;
        public static PdfPTable _getInfosGarage()
        {
            PdfPTable tableInfosGarage = new PdfPTable(1);
            tableInfosGarage.WidthPercentage = 80;
            tableInfosGarage.DefaultCell.Border = Rectangle.NO_BORDER;

            var fontColor = new BaseColor(224, 52, 48);

            string adresse = "";
            string telephones = "";
            string email = "";
            byte[] logo = null;
            byte[] entete = null;
            if (Parametrage != null)
            {
                if (Parametrage.Adresse != null)
                {
                    adresse = Parametrage.Adresse.Ligne1;
                    adresse += !string.IsNullOrEmpty(Parametrage.Adresse.Ligne2) ? "\n" + Parametrage.Adresse.Ligne2 : "";
                    adresse += !string.IsNullOrEmpty(Parametrage.Adresse.Ligne3) ? "\n" + Parametrage.Adresse.Ligne3 : "";
                    adresse += "\n" + Parametrage.Adresse.CodePostal + " " + Parametrage.Adresse.Ville;
                }
                if (!string.IsNullOrEmpty(Parametrage.TelephoneEntreprise))
                    telephones += Parametrage.TelephoneEntreprise;
                if (!string.IsNullOrEmpty(Parametrage.TelephoneEntreprise) && !string.IsNullOrEmpty(Parametrage.PortableEntreprise))
                    telephones += " - ";
                if (!string.IsNullOrEmpty(Parametrage.PortableEntreprise))
                    telephones += Parametrage.PortableEntreprise;
                if (!string.IsNullOrEmpty(Parametrage.EmailEntreprise))
                    email = Parametrage.EmailEntreprise;
                if (Parametrage.Logo != null)
                    logo = Parametrage.Logo;
                if (Parametrage.Entete != null)
                    entete = Parametrage.Entete;
            }

            if (entete == null)
            {
                PdfPTable tableGauche = new PdfPTable(1);
                tableGauche.WidthPercentage = 100;
                tableGauche.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell cellInfoGarage = new PdfPCell()
                {
                    Border = 0,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };
                PdfPCell cellLogo = new PdfPCell()
                {
                    Border = 0,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    UseAscender = true
                };
                PdfPCell cellDeuxCols = new PdfPCell()
                {
                    Border = 0,
                    Colspan = 2,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                };

                Paragraph paraEntete = new Paragraph(adresse + "\n" + telephones);
                paraEntete.Alignment = Element.ALIGN_CENTER;
                paraEntete.Font.Color = fontColor;
                paraEntete.Font.SetStyle("bold");

                if (logo != null)
                {
                    iTextSharp.text.Image imgLogo = iTextSharp.text.Image.GetInstance(logo);
                    imgLogo.WidthPercentage = 100;
                    imgLogo.Alignment = 2;
                    cellLogo.AddElement(imgLogo);
                    tableGauche.AddCell(cellLogo);
                    cellLogo.CompositeElements.Clear();
                }
                
                cellDeuxCols.AddElement(paraEntete);
                tableGauche.AddCell(cellDeuxCols);

                cellInfoGarage.AddElement(tableGauche);
                tableInfosGarage.AddCell(cellInfoGarage);
            }
            else
            {
                PdfPTable tableGauche = new PdfPTable(1);
                tableGauche.WidthPercentage = 100;
                tableGauche.DefaultCell.Border = Rectangle.NO_BORDER;

                PdfPCell cellLogo = new PdfPCell()
                {
                    Border = Rectangle.NO_BORDER,
                    VerticalAlignment = Element.ALIGN_MIDDLE
                };

                iTextSharp.text.Image imgLogo = iTextSharp.text.Image.GetInstance(entete);
                imgLogo.WidthPercentage = 100;
                imgLogo.Alignment = 2;
                imgLogo.Border = 25;
                cellLogo.AddElement(imgLogo);
                tableGauche.AddCell(cellLogo);
                
                tableInfosGarage.AddCell(tableGauche);
            }

            return tableInfosGarage;
        }
        public static PdfPTable _getInfosFacture(PieceVente pPieceVente)
        {
            PdfPTable tableFacture = new PdfPTable(2);
            tableFacture.SpacingBefore = 10f;

            PdfPCell cellTableFacture = new PdfPCell()
            {
                Padding = 3,
                VerticalAlignment = 1
            };

            Font fontTitle = new Font(Font.FontFamily.UNDEFINED, 12, Font.BOLD);
            Font fontSmall = new Font(Font.FontFamily.UNDEFINED, 9, Font.NORMAL);

            if (pPieceVente.NumFacture != null)
                cellTableFacture.Phrase = new Phrase("FACTURE N°", fontTitle);
            else
                cellTableFacture.Phrase = new Phrase("DEVIS N°", fontTitle);
            tableFacture.AddCell(cellTableFacture);
            cellTableFacture.Phrase = new Phrase("DATE", fontTitle);
            tableFacture.AddCell(cellTableFacture);

            int numPieceVente = 0;
            DateTime datePieceVente = DateTime.Now;
            if (pPieceVente.NumFacture != null)
            {
                numPieceVente = pPieceVente.NumFacture.Value;
                datePieceVente = pPieceVente.DateFacture.Value;
            }
            else
            {
                numPieceVente = pPieceVente.NumDevis.Value;
                datePieceVente = pPieceVente.DateDevis.Value;
            }
            cellTableFacture.Phrase = new Phrase(numPieceVente.ToString(), fontSmall);
            tableFacture.AddCell(cellTableFacture);
            cellTableFacture.Phrase = new Phrase(datePieceVente.ToShortDateString(), fontSmall);
            tableFacture.AddCell(cellTableFacture);

            return tableFacture;
        }
        public static PdfPTable _getInfosClient(PieceVente pPieceVente)
        {
            PdfPTable tableIdentificationDuClient = new PdfPTable(2);
            tableIdentificationDuClient.SpacingBefore = 10f;

            PdfPCell cellTableIdentificationDuClient = new PdfPCell()
            {
                Padding = 3,
                VerticalAlignment = 1
            };

            Font fontCourierSmall = new Font(Font.FontFamily.UNDEFINED, 9, Font.NORMAL);

            cellTableIdentificationDuClient.Phrase = new Phrase("Nom :", fontCourierSmall);
            tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
            cellTableIdentificationDuClient.Phrase = new Phrase(pPieceVente.Client.Nom, fontCourierSmall);
            tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);

            if (pPieceVente.Client.Telephone != "" && pPieceVente.Client.Telephone != "0")
            {
                cellTableIdentificationDuClient.Phrase = new Phrase("Téléphone :", fontCourierSmall);
                tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
                cellTableIdentificationDuClient.Phrase = new Phrase(pPieceVente.Client.Telephone, fontCourierSmall);
                tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
            }

            cellTableIdentificationDuClient.Phrase = new Phrase("Voiture :", fontCourierSmall);
            tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
            cellTableIdentificationDuClient.Phrase = new Phrase(pPieceVente.Voiture.Modele.Marque.Libelle + " " + pPieceVente.Voiture.Modele.Libelle, fontCourierSmall);
            tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);

            if (pPieceVente.Voiture.Immatriculation != "" && pPieceVente.Voiture.Immatriculation != "0")
            {
                cellTableIdentificationDuClient.Phrase = new Phrase("Immatriculation :", fontCourierSmall);
                tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
                cellTableIdentificationDuClient.Phrase = new Phrase(pPieceVente.Voiture.Immatriculation, fontCourierSmall);
                tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
            }

            if (pPieceVente.Kilometrage > 1)
            {
                cellTableIdentificationDuClient.Phrase = new Phrase("Kilométrage :", fontCourierSmall);
                tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
                cellTableIdentificationDuClient.Phrase = new Phrase(pPieceVente.Kilometrage.ToString() + "km", fontCourierSmall);
                tableIdentificationDuClient.AddCell(cellTableIdentificationDuClient);
            }

            return tableIdentificationDuClient;
        }
        public static PdfPTable GetEntete(PieceVente pPieceVente)
        {
            PdfPTable tableEntete = new PdfPTable(2);
            tableEntete.SpacingBefore = 10f;
            tableEntete.WidthPercentage = 100;
            PdfPCell cellTableEntete = new PdfPCell()
            {
                Border = 0,
                Padding = 8,
                VerticalAlignment = 1
            };

            cellTableEntete.AddElement(_getInfosGarage());
            tableEntete.AddCell(cellTableEntete);

            cellTableEntete.CompositeElements.Clear();
            cellTableEntete.AddElement(_getInfosFacture(pPieceVente));
            cellTableEntete.AddElement(_getInfosClient(pPieceVente));
            tableEntete.AddCell(cellTableEntete);

            return tableEntete;
        }
        public static PdfPTable GetTableauReparations(PieceVente pPieceVente)
        {
            PdfPTable tableReparations = new PdfPTable(4);
            tableReparations.SpacingBefore = 40f;
            tableReparations.WidthPercentage = 100;
            float[] widths = new float[] { 50f, 10f, 20f, 20f };
            tableReparations.SetWidths(widths);
            PdfPCell cellTableHeaderReparations = new PdfPCell()
            {
                Padding = 8,
                VerticalAlignment = 1,
                HorizontalAlignment = 1
            };
            PdfPCell cellTableReparations = new PdfPCell()
            {
                Padding = 8,
                VerticalAlignment = 1,
                HorizontalAlignment = 1
            };
            PdfPCell cellSansBordure = new PdfPCell()
            {
                Padding = 8,
                Border = 0,
                Phrase = new Phrase(" ")
            };
            cellTableHeaderReparations.Phrase = new Phrase("Désignation", new Font(Font.FontFamily.UNDEFINED, 12f, 1));
            tableReparations.AddCell(new PdfPCell(cellTableHeaderReparations));
            cellTableHeaderReparations.Phrase = new Phrase("Qté.", new Font(Font.FontFamily.UNDEFINED, 12f, 1));
            tableReparations.AddCell(new PdfPCell(cellTableHeaderReparations));
            cellTableHeaderReparations.Phrase = new Phrase("PU HT", new Font(Font.FontFamily.UNDEFINED, 12f, 1));
            tableReparations.AddCell(new PdfPCell(cellTableHeaderReparations));
            cellTableHeaderReparations.Phrase = new Phrase("Total HT", new Font(Font.FontFamily.UNDEFINED, 12f, 1));
            tableReparations.AddCell(new PdfPCell(cellTableHeaderReparations));

            foreach (var ligne in pPieceVente.Lignes)
            {
                cellTableReparations.Phrase = new Phrase(ligne.Libelle != null ? ligne.Libelle.ToUpper() : "", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase(ligne.Quantite.ToString(), new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase(ligne.PrixClientHT.ToString("0.00") + " €", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase((ligne.PrixClientHT * ligne.Quantite).ToString("0.00") + " €", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
            }
            foreach (var service in pPieceVente.Services)
            {
                cellTableReparations.Phrase = new Phrase(service.Libelle != null ? service.Libelle.ToUpper() : "", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase(service.Quantite.ToString(), new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase(service.PrixClientHT.ToString("0.00") + " €", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase((service.PrixClientHT * service.Quantite).ToString("0.00") + " €", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
            }

            if (pPieceVente.MainDOeuvreMontantHoraire > 0)
            {
                cellTableReparations.Phrase = new Phrase("MAIN D'OEUVRE", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase(pPieceVente.MainDOeuvreDuree.ToString(), new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase((pPieceVente.MainDOeuvreMontantHoraire / (0.2F + 1)).ToString("0.00") + " €", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
                cellTableReparations.Phrase = new Phrase((pPieceVente.MainDOeuvreDuree * (pPieceVente.MainDOeuvreMontantHoraire / (0.2F + 1))).ToString("0.00") + " €", new Font(Font.FontFamily.UNDEFINED, 9f, 0));
                tableReparations.AddCell(cellTableReparations);
            }

            for (int i = 0; i < 10 - pPieceVente.Lignes.Count; i++)
            {
                cellTableReparations.Phrase = new Phrase(" ");
                tableReparations.AddCell(cellTableReparations);
                tableReparations.AddCell(cellTableReparations);
                tableReparations.AddCell(cellTableReparations);
                tableReparations.AddCell(cellTableReparations);
            }

            if (pPieceVente.Remise != null && pPieceVente.Remise > 0)
            {
                tableReparations.AddCell(cellSansBordure);
                tableReparations.AddCell(cellSansBordure);
                cellTableHeaderReparations.Phrase = new Phrase("Remise");
                tableReparations.AddCell(cellTableHeaderReparations);
                cellTableReparations.Phrase = new Phrase(pPieceVente.Remise.Value.ToString("0.00") + " %");
                tableReparations.AddCell(cellTableReparations);
            }

            tableReparations.AddCell(cellSansBordure);
            tableReparations.AddCell(cellSansBordure);
            cellTableHeaderReparations.Phrase = new Phrase("Total HT");
            tableReparations.AddCell(cellTableHeaderReparations);
            cellTableReparations.Phrase = new Phrase(pPieceVente.TotalHT.ToString("0.00") + " €");
            tableReparations.AddCell(cellTableReparations);

            tableReparations.AddCell(cellSansBordure);
            tableReparations.AddCell(cellSansBordure);
            cellTableHeaderReparations.Phrase = new Phrase("Taux TVA");
            tableReparations.AddCell(cellTableHeaderReparations);
            cellTableReparations.Phrase = new Phrase((0.2F * 100).ToString("0.00") + " %");
            tableReparations.AddCell(cellTableReparations);

            tableReparations.AddCell(cellSansBordure);
            tableReparations.AddCell(cellSansBordure);
            cellTableHeaderReparations.Phrase = new Phrase("Montant TVA");
            tableReparations.AddCell(cellTableHeaderReparations);
            cellTableReparations.Phrase = new Phrase(pPieceVente.MontantTVA.ToString("0.00") + " €");
            tableReparations.AddCell(cellTableReparations);

            tableReparations.AddCell(cellSansBordure);
            tableReparations.AddCell(cellSansBordure);
            cellTableHeaderReparations.Phrase = new Phrase("Total TTC");
            tableReparations.AddCell(cellTableHeaderReparations);
            cellTableReparations.Phrase = new Phrase(pPieceVente.TotalTTC.ToString("0.00") + " €");
            tableReparations.AddCell(cellTableReparations);

            return tableReparations;
        }
        public static List<Rectangle> GetFooter(PieceVente pPieceVente)
        {
            List<Rectangle> footer = new List<Rectangle>();

            Rectangle rectGauche = new Rectangle(300, 150, 50, 50);
            rectGauche.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
            rectGauche.BorderWidth = 1;
            rectGauche.BorderColor = new BaseColor(0, 0, 0);
            footer.Add(rectGauche);

            Rectangle rectDroite = new Rectangle(550, 150, 300, 50);
            rectDroite.Border = Rectangle.LEFT_BORDER | Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
            rectDroite.BorderWidth = 1;
            rectDroite.BorderColor = new BaseColor(0, 0, 0);

            footer.Add(rectDroite);

            return footer;
        }
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

        public class PDFFooter : PdfPageEventHelper
        {
            public PieceVente pieceVente;
            public Parametrage parametrage;

            public PDFFooter(PieceVente pPieceVente, Parametrage pParametrage) : base()
            {
                this.pieceVente = pPieceVente;
                this.parametrage = pParametrage;
            }

            // write on top of document
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                base.OnOpenDocument(writer, document);
                /*PdfPTable tabFot = new PdfPTable(new float[] { 1F });
                tabFot.SpacingAfter = 10F;
                PdfPCell cell;
                tabFot.TotalWidth = 300F;
                cell = new PdfPCell(new Phrase("Header"));
                tabFot.AddCell(cell);
                tabFot.WriteSelectedRows(0, -1, 150, document.Top, writer.DirectContent);*/
            }

            // write on start of each page
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);
            }

            // write on end of each page
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                PdfPTable tabFot = new PdfPTable(1);
                PdfPCell cell;

                tabFot.TotalWidth = 300F;

                string footer1 = "";
                if (!string.IsNullOrEmpty(parametrage.SIRET))
                    footer1 += "SIRET : " + parametrage.SIRET + " - ";
                if (!string.IsNullOrEmpty(parametrage.CodeAPE))
                    footer1 += "Code APE : " + parametrage.CodeAPE + " - ";
                if (!string.IsNullOrEmpty(parametrage.TVAIntraCommunautaire))
                    footer1 += "N° TVA : " + parametrage.TVAIntraCommunautaire + " - ";
                if (footer1.Any())
                    footer1 = footer1.Substring(0, footer1.Length - 2);

                string footer2 = "";
                if (!string.IsNullOrEmpty(parametrage.LibelleBasDePage))
                    footer2 = parametrage.LibelleBasDePage;

                Phrase phraseFooter1 = new Phrase(footer1);
                phraseFooter1.Font.Size = 7;
                phraseFooter1.Font.SetStyle("italic");

                Phrase phraseFooter2 = new Phrase(footer2);
                phraseFooter2.Font.Size = 7;
                phraseFooter2.Font.SetStyle("italic");
                
                cell = new PdfPCell(phraseFooter1);
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                tabFot.AddCell(cell);
                cell = new PdfPCell(phraseFooter2);
                cell.Border = 0;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                tabFot.AddCell(cell);
                tabFot.WriteSelectedRows(0, -1, 150, document.Bottom, writer.DirectContent);
            }

            //write on close of document
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
            }
        }
    }
}