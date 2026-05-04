using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.Code;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.DAL.Parametrage;
using CEDAlfaiaApp.Models;
using CEDAlfaiaApp.Reports;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CEDAlfaiaApp.Controllers
{
    public class DevisController : CEDAlfaiaAppControllerBase
    {

        [HttpGet]
        public ActionResult Liste(string idClient)
        {
            ViewBag.UserID = this.UserID.Value;
            ViewBag.ClientID = idClient;
            return View();
        }

        [HttpGet]
        public ActionResult Liste_old(string idClient)
        {
            int id = 0;
            if (idClient != null)
                int.TryParse(idClient, out id);
            var devis = CEDAlfaiaAppWebService.GetService().GarageService.GetAllDevis();
            if (id > 0)
                devis = devis.Where(d => d.ClientID == id).ToList();
            ViewBag.UserID = this.UserID.Value;
            return View(devis.Select(d => new DevisModel(d)).OrderByDescending(d => d.Date.Year).ThenByDescending(d => d.Date.Month).ThenByDescending(d => d.Date.Day).ToList());
        }

        [ValidateInput(false)]
        public ActionResult GridViewListeDevisPartial(int? idClient)
        {
            return PartialView("_GridViewListeDevisPartial", CEDAlfaiaAppWebService.GetService().GarageService.Devis().Where(d => d.ClientID == (idClient != null ? idClient : d.ClientID)));
        }

        [HttpGet]
        public FileResult ImprimerParLot(string ids)
        {
            /*List<int> listIds = new JavaScriptSerializer().Deserialize<List<int>>(ids);

            List<PieceVente> devis = new List<PieceVente>();
            if (listIds.Any())
            {
                string nomFichier = "Devis " + DateTime.Now.ToShortDateString() + ".pdf";
                List<byte[]> listArray = new List<byte[]>();
                foreach (int id in listIds)
                {
                    PieceVente devisFound = CEDAlfaiaAppWebService.GetService().GarageService.GetDevis(id);
                    if (devisFound != null)
                        listArray.Add(editerDevis(devisFound));
                }
                return File(Impression.JoinArrays(listArray), System.Net.Mime.MediaTypeNames.Application.Octet, nomFichier);
            }*/

            return null;
        }

        [HttpGet]
        public ActionResult Saisir(string identifiant, string typeId = null)
        {
            int id = 0;
            int.TryParse(identifiant, out id);

            int? idClient = null;
            int? idDevis = null;
            if (typeId == null)
                typeId = "0";
            switch (typeId)
            {
                // devis
                case "0":
                    idDevis = id;
                    break;
                // client
                case "1":
                    idClient = id;
                    break;
            }

            if (idDevis != null && idDevis > 0)
            {
                PieceVente devis = CEDAlfaiaAppWebService.GetService().GarageService.GetDevis(id);

                return View(new SaisirDevisModel(devis));
            }

            float montantMainDOeuvre = 0;
            if (this.UserID != null)
            {
                Parametrage parametrage = CEDAlfaiaAppWebService.GetService().ParametrageService.GetParametrageUtilisateur(this.UserID.Value);
                montantMainDOeuvre = parametrage.MainDOeuvreMontantHoraire;
            }
            return View(new SaisirDevisModel()
            {
                DureeMainDOeuvre = 1,
                MainDOeuvre = montantMainDOeuvre
            });
        }

        [HttpPost]
        public ActionResult Saisir(SaisirDevisModel pDevis)
        {
            if (!ModelState.IsValid)
            {
                return View(pDevis);
            }

            if (pDevis.Date == null)
            {
                ModelState.AddModelError("", "La date du devis est obligatoire !");
                return View(pDevis);
            }

            try
            {
                return saisirDevis(pDevis);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Le devis n'a pas pu être enregistré : " + ex.Message);
                return View(pDevis);
            }
        }

        private ActionResult saisirDevis(SaisirDevisModel pDevis)
        {
            if (pDevis != null)
            {
                float tauxTVA = 1;
                if (this.UtilisateurEnCours.Entreprise != null && this.UtilisateurEnCours.Entreprise.Parametrage != null)
                    tauxTVA = this.UtilisateurEnCours.Entreprise.Parametrage.TVA;

                int? idRdv = null;
                if (pDevis.RendezVous != null && !string.IsNullOrEmpty(pDevis.RendezVous.Sujet))
                {
                    RendezVous rdv = new RendezVous();
                    DateTime dateHeureDebut = DateTime.Parse(pDevis.RendezVous.DateDebut);
                    if (pDevis.RendezVous.HeureDebut.Split(':').Count() > 1)
                    {
                        int hours = int.Parse(pDevis.RendezVous.HeureDebut.Split(':').First());
                        int minutes = int.Parse(pDevis.RendezVous.HeureDebut.Split(':')[1]);
                        dateHeureDebut.AddHours(hours);
                        dateHeureDebut.AddMinutes(minutes);
                    }
                    DateTime dateHeureFin = DateTime.Parse(pDevis.RendezVous.DateFin);
                    if (pDevis.RendezVous.HeureFin.Split(':').Count() > 1)
                    {
                        int hours = int.Parse(pDevis.RendezVous.HeureFin.Split(':').First());
                        int minutes = int.Parse(pDevis.RendezVous.HeureFin.Split(':')[1]);
                        dateHeureFin.AddHours(hours);
                        dateHeureFin.AddMinutes(minutes);
                    }
                    rdv = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateRendezVous(pDevis.RendezVous.ID, pDevis.RendezVous.Sujet, pDevis.RendezVous.Duree, dateHeureDebut, dateHeureFin, pDevis.RendezVous.Commentaire);
                    idRdv = rdv.ID;
                }

                // enregistrer devis
                PieceVente devis = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDevis(pDevis.IDDevis, pDevis.IDVoiture, DateTime.Parse(pDevis.Date), pDevis.MainDOeuvre, pDevis.DureeMainDOeuvre, pDevis.Kilometrage, pDevis.Remise, idRdv);

                // enregistrer lignes devis
                foreach (var ligne in pDevis.Lignes)
                {
                    if (!ligne.IsEmpty && !ligne.IsSupprime)
                        CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateLigneDevis(ligne.IDLigne, devis.ID, null, ligne.Libelle, ligne.PrixGarageHT, ligne.PrixGarageHT + (ligne.PrixGarageHT * tauxTVA), ligne.PrixGarageHT / (1 - (ligne.Remise / 100)), ligne.PrixGarageHT / (1 - (ligne.Remise / 100)) + (ligne.PrixGarageHT / (1 - (ligne.Remise / 100)) * tauxTVA), ligne.Quantite, ligne.Remise);
                    else if (!ligne.IsEmpty && ligne.IDLigne > 0)
                        CEDAlfaiaAppWebService.GetService().GarageService.DeleteLigneDevis(ligne.IDLigne);
                }

                // enregistrer lignes services
                foreach (var service in pDevis.Services)
                {
                    if (!service.IsEmpty && !service.IsSupprime)
                        CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateServicePieceVente(service.IDService, devis.ID, service.Libelle, service.PrixClientTTC / 1.2F, service.PrixClientTTC, service.Quantite);
                    else if (!service.IsEmpty && service.IDService > 0)
                        CEDAlfaiaAppWebService.GetService().GarageService.DeleteServicePieceVente(service.IDService);
                }

                // enregistrer Devis Fournisseur
                byte[] devisFournisseur = null;
                string devisFournisseurFormatFichier = "";
                if (!string.IsNullOrEmpty(pDevis.DevisFornisseurBase64) && pDevis.DevisFornisseurBase64.Contains(','))
                {
                    devisFournisseurFormatFichier = pDevis.DevisFornisseurBase64.Split(',').First();
                    devisFournisseur = Convert.FromBase64String(pDevis.DevisFornisseurBase64.Split(',').Last());

                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(devis.ID, TypeDocument.DevisFournisseur, "Devis fournisseur", devisFournisseur, devisFournisseurFormatFichier);
                }

                // enregistrer BL Fournisseur
                byte[] blFournisseur = null;
                string blFournisseurFormatFichier = "";
                if (!string.IsNullOrEmpty(pDevis.BonLivraisonFornisseurBase64) && pDevis.BonLivraisonFornisseurBase64.Contains(','))
                {
                    blFournisseurFormatFichier = pDevis.BonLivraisonFornisseurBase64.Split(',').First();
                    blFournisseur = Convert.FromBase64String(pDevis.BonLivraisonFornisseurBase64.Split(',').Last());

                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(devis.ID, TypeDocument.BonLivraisonFournisseur, "Bon livraison fournisseur", blFournisseur, blFournisseurFormatFichier);
                }

                // enregistrer paiements
                /*foreach (var paiement in pDevis.Paiements)
                {
                    if (!paiement.IsEmpty && !paiement.IsSupprime)
                    {
                        if (paiement.IDPaiement != 0 || paiement.Montant > 0)
                            CEDAlfaiaAppWebService.GetService().GarageService.AddUpdatePieceVentePaiement(paiement.IDPaiement, devis.ID, paiement.Date, paiement.Montant, paiement.TypePaiement);
                    }
                    else if (!paiement.IsEmpty && paiement.IDPaiement > 0)
                        CEDAlfaiaAppWebService.GetService().GarageService.DeletePaiementPieceVente(paiement.IDPaiement);
                }*/

                // vider le le fichier de la base de données
                DocumentPieceVente docDevis = devis.Documents.Where(d => d.PieceVenteID == devis.ID && d.TypeDocument == TypeDocument.Devis).FirstOrDefault();
                if (docDevis != null)
                    CEDAlfaiaAppWebService.GetService().GarageService.DeleteDocument(docDevis.ID);

                CEDAlfaiaAppWebService.GetService().GarageService.UpdateTotauxPieceVente(devis.ID);

                return RedirectToAction("Liste");
            }
            return View();
        }
        [HttpGet]
        public FileResult Editer(int idDevis)
        {
            PieceVente devisFound = CEDAlfaiaAppWebService.GetService().GarageService.GetDevis(idDevis);
            if (devisFound != null)
            {
                XtraReport report = new DevisXtraReport();
                using (MemoryStream ms = new MemoryStream())
                {
                    var dataSource = report.DataSource as EFDataSource;
                    dataSource.Filters["PiecesVente"] = "ID = " + devisFound.ID;
                    report.DataSource = dataSource;
                    report.ExportToPdf(ms);
                    return ExportDocument(ms.ToArray(), "pdf", "Devis " + devisFound.NumDevis.Value + ".pdf", true);
                }
            }
            return null;
        }

        private FileResult ExportDocument(byte[] document, string format, string fileName, bool isInline)
        {
            string contentType;
            string disposition = (isInline) ? "inline" : "attachment";

            switch (format.ToLower())
            {
                case "docx":
                    contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                    break;
                case "xls":
                    contentType = "application/vnd.ms-excel";
                    break;
                case "xlsx":
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    break;
                case "mht":
                    contentType = "message/rfc822";
                    break;
                case "html":
                    contentType = "text/html";
                    break;
                case "txt":
                case "csv":
                    contentType = "text/plain";
                    break;
                case "png":
                    contentType = "image/png";
                    break;
                default:
                    contentType = String.Format("application/{0}", format);
                    break;
            }

            Response.AddHeader("Content-Disposition", String.Format("{0}; filename={1}", disposition, fileName));
            return File(document, contentType);
        }

        /*public byte[] editerDevis(PieceVente devis)
        {
            byte[] fichier = null;
            Parametrage parametrage = null;
            if (this.UserID != null)
                parametrage = CEDAlfaiaAppWebService.GetService().ParametrageService.GetParametrageUtilisateur(this.UserID.Value);

            if (parametrage != null && parametrage.IsSavePieceDeVente && devis.Documents.Any(d => d.TypeDocument == TypeDocument.Devis))
                fichier = devis.Documents.Where(d => d.TypeDocument == TypeDocument.Devis).First().Doc;

            if (fichier == null)
            {
                string cheminFichier = _createPDF(devis, parametrage);
                fichier = System.IO.File.ReadAllBytes(cheminFichier);
                if (parametrage != null && parametrage.IsSavePieceDeVente)
                {
                    DocumentPieceVente docPV = new DocumentPieceVente(devis.ID, "Devis " + devis.NumDevis.Value, TypeDocument.Devis, fichier, null);
                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(docPV);
                }
            }

            return fichier;
        }*/


        [HttpPost]
        public ActionResult UploadFiles()
        {
            // Checking no of files injected in Request object  
            if (Request.Files.Count > 0)
            {
                try
                {
                    //  Get all files from Request object  
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                        //string filename = Path.GetFileName(Request.Files[i].FileName);  

                        HttpPostedFileBase file = files[i];
                        string fname;

                        // Checking for Internet Explorer  
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }

                        // Get the complete folder path and store the file inside it.  
                        fname = Path.Combine(Server.MapPath("~/Uploads/"), fname);
                        file.SaveAs(fname);
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }
        }

        #region Impression
        /*private string _createPDF(PieceVente pDevis, Parametrage pParametrage)
        {
            Document document = new Document(PageSize.A4);
            string appRootDir = ControllerContext.HttpContext.Server.MapPath("~");
            try
            {
                var nomFacture = pDevis.Client.Nom + " " + pDevis.NumDevis;
                var dossierFichier = "Devis";
                var filePath = appRootDir + "\\" + dossierFichier + "\\" + nomFacture + ".pdf";
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (Document doc = new Document(PageSize.A4, 36, 36, 36, 36))
                using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                {
                    writer.PageEvent = new Impression.PDFFooter(pDevis, pParametrage);

                    doc.Open();

                    doc.Add(Impression.GetEntete(pDevis));

                    doc.Add(Impression.GetTableauReparations(pDevis));

                    List<Rectangle> rectangles = Impression.GetFooter(pDevis);
                    foreach (Rectangle rect in rectangles)
                    {
                        doc.Add(rect);
                    }

                    PdfContentByte cb = writer.DirectContent;

                    ColumnText ctEntreprise = new ColumnText(cb);
                    ColumnText ctClient = new ColumnText(cb);
                    Paragraph p = new Paragraph();

                    ctEntreprise.SetSimpleColumn(rectangles.First());
                    p.Add(new Chunk("  Cachet de l'entreprise", new Font(Font.FontFamily.UNDEFINED, 10, Font.ITALIC)));
                    ctEntreprise.AddText(p);
                    ctEntreprise.Go();

                    ctClient.SetSimpleColumn(rectangles.Last());
                    p.Clear();
                    if (pDevis.NumFacture != null)
                        p.Add(new Chunk("  Observations", new Font(Font.FontFamily.UNDEFINED, 10, Font.ITALIC)));
                    else
                        p.Add(new Chunk("  Signature du client", new Font(Font.FontFamily.UNDEFINED, 10, Font.ITALIC)));

                    ctClient.AddText(p);
                    ctClient.Go();

                    // Step 6: Closing the Document
                    doc.Close();
                }
                return filePath;
            }
            // Catching iTextSharp.text.DocumentException if any
            catch (DocumentException de)
            {
                throw de;
            }
            // Catching System.IO.IOException if any
            catch (IOException ioe)
            {
                throw ioe;
            }
        }

        private string _createPDF(List<PieceVente> pDevis)
        {
            Document document = new Document(PageSize.A4);
            string appRootDir = ControllerContext.HttpContext.Server.MapPath("~");
            try
            {
                var nomFacture = "Devis - " + DateTime.Now.ToShortDateString();
                var dossierFichier = "Devis";
                var filePath = appRootDir + "\\" + dossierFichier + "\\" + nomFacture + ".pdf";
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (Document doc = new Document(PageSize.A4, 36, 36, 36, 36))
                using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                {
                    doc.Open();

                    foreach (PieceVente devisScan in pDevis)
                    {
                        doc.Add(Impression.GetEntete(devisScan));

                        doc.Add(Impression.GetTableauReparations(devisScan));

                        List<Rectangle> rectangles = Impression.GetFooter(devisScan);
                        foreach (Rectangle rect in rectangles)
                        {
                            doc.Add(rect);
                        }

                        PdfContentByte cb = writer.DirectContent;

                        ColumnText ctEntreprise = new ColumnText(cb);
                        ColumnText ctClient = new ColumnText(cb);
                        Paragraph p = new Paragraph();

                        ctEntreprise.SetSimpleColumn(rectangles.First());
                        p.Add(new Chunk("  Cachet de l'entreprise", new Font(Font.FontFamily.UNDEFINED, 10, Font.ITALIC)));
                        ctEntreprise.AddText(p);
                        ctEntreprise.Go();

                        ctClient.SetSimpleColumn(rectangles.Last());
                        p.Clear();
                        if (devisScan.NumFacture != null)
                            p.Add(new Chunk("  Observations", new Font(Font.FontFamily.UNDEFINED, 10, Font.ITALIC)));
                        else
                            p.Add(new Chunk("  Signature du client", new Font(Font.FontFamily.UNDEFINED, 10, Font.ITALIC)));

                        ctClient.AddText(p);
                        ctClient.Go();

                        doc.NewPage();
                    }
                    
                    // Step 6: Closing the Document
                    doc.Close();
                }
                return filePath;
            }
            // Catching iTextSharp.text.DocumentException if any
            catch (DocumentException de)
            {
                throw de;
            }
            // Catching System.IO.IOException if any
            catch (IOException ioe)
            {
                throw ioe;
            }
        }
        */
        #endregion

        /*[HttpPost]
        public string SupprimerDevis(string idDevis)
        {
            try
            {
                int id = 0;
                int.TryParse(idDevis, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.DeleteDevis(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string GenererFacture(string idDevis)
        {
            try
            {
                int id = 0;
                int.TryParse(idDevis, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.GenererFactureByIDDevis(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }*/
        [HttpGet]
        public ActionResult GenererFacture(string idDevis)
        {
            try
            {
                int id = 0;
                int.TryParse(idDevis, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.GenererFactureByIDDevis(id);

                return RedirectToAction("Liste");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Liste");
            }
        }

        [HttpGet]
        public ActionResult SupprimerDevis(string idDevis)
        {
            try
            {
                int id = 0;
                int.TryParse(idDevis, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.DeleteDevis(id);
                return RedirectToAction("Liste");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Liste");
            }
        }

        [HttpGet]
        public async Task<ActionResult> LigneDevisChange(string idLigne, string libelle, string quantite, string remise, string prixGarageHT, string indexLigne, string isEmpty, string isSupprime)
        {
            int id = int.Parse(idLigne);
            int quantiteSaisie = int.Parse(quantite);
            float remiseSaisie = float.Parse(remise);
            float prixHTSaisie = float.Parse(prixGarageHT);
            int indexLigneSaisie = int.Parse(indexLigne);
            bool isSupprimeSaisie = bool.Parse(isSupprime);
            bool isEmptySaisie = bool.Parse(isEmpty);

            float tauxTVA = 1;
            if (this.UtilisateurEnCours.Entreprise != null && this.UtilisateurEnCours.Entreprise.Parametrage != null)
                tauxTVA = this.UtilisateurEnCours.Entreprise.Parametrage.TVA;

            var model = new SaisirLignesDevisModel(id, libelle, quantiteSaisie, remiseSaisie, prixHTSaisie, prixHTSaisie + (prixHTSaisie * tauxTVA), prixHTSaisie / (1 - (remiseSaisie / 100)), prixHTSaisie / (1 - (remiseSaisie / 100)) + (prixHTSaisie / (1 - (remiseSaisie / 100)) * tauxTVA), indexLigneSaisie, isSupprimeSaisie, isEmptySaisie);

            return PartialView("_LigneDevis", model);
        }
        [HttpGet]
        public async Task<ActionResult> LigneDevisAdd(string indexLigne)
        {
            int indexLigneSaisie = int.Parse(indexLigne);

            var model = new SaisirLignesDevisModel();
            model.IndexLigne = indexLigneSaisie;

            return PartialView("_LigneDevis", model);
        }
        [HttpGet]
        public async Task<ActionResult> PaiementAdd(string indexLigne)
        {
            int indexLigneSaisie = int.Parse(indexLigne);

            var model = new SaisirPaiementPieceVenteModel();
            //model.IndexLigne = indexLigneSaisie;
            model.Date = DateTime.Now;

            return PartialView("_Paiement", model);
        }
        [HttpGet]
        public async Task<ActionResult> ServicesChange(string idService, string libelle, string quantite, string prixTTC, string indexService, string isEmpty, string isSupprime)
        {
            int id = int.Parse(idService);
            int quantiteSaisie = int.Parse(quantite);
            float prixTTCSaisie = float.Parse(prixTTC);
            int indexServiceSaisie = int.Parse(indexService);
            bool isSupprimeSaisie = bool.Parse(isSupprime);
            bool isEmptySaisie = bool.Parse(isEmpty);

            float tauxTVA = 1;
            if (this.UtilisateurEnCours.Entreprise != null && this.UtilisateurEnCours.Entreprise.Parametrage != null)
                tauxTVA = this.UtilisateurEnCours.Entreprise.Parametrage.TVA;

            var model = new SaisirServicesModel(id, libelle, quantiteSaisie, prixTTCSaisie / (tauxTVA + 1), prixTTCSaisie, indexServiceSaisie, isSupprimeSaisie, isEmptySaisie);

            return PartialView("_LigneService", model);
        }
        [HttpGet]
        public async Task<ActionResult> ServicesAdd(string indexService)
        {
            int indexServiceSaisie = int.Parse(indexService);

            var model = new SaisirServicesModel();
            model.IndexService = indexServiceSaisie;

            return PartialView("_LigneService", model);
        }
    }
}