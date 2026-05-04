using Newtonsoft.Json;
using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.Code;
using CEDAlfaiaApp.Core;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using CEDAlfaiaApp.DAL.Parametrage;
using System.Net.Mail;
using DevExpress.XtraReports.UI;
using CEDAlfaiaApp.Reports;
using DevExpress.DataAccess.EntityFramework;

namespace CEDAlfaiaApp.Controllers
{
    public class FacturesController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Liste(string idClient, DateTime? dateDebut, DateTime? dateFin)
        {
            int? client = null;
            if (!string.IsNullOrEmpty(idClient))
                client = Convert.ToInt32(idClient);

            if (dateDebut == null)
                dateDebut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (dateFin == null)
                dateFin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            ViewBag.UserID = this.UserID.Value;

            return View(new SearchPieceVenteModel()
            {
                ClientID = client,
                DateDebut = dateDebut.Value,
                DateFin = dateFin.Value
            });
        }

        [HttpGet]
        public ActionResult Liste_old(string idClient)
        {
            int id = 0;
            if (idClient != null)
                int.TryParse(idClient, out id);
            var factures = CEDAlfaiaAppWebService.GetService().GarageService.GetFactures();
            if (id > 0)
                factures = factures.Where(d => d.ClientID == id).ToList();
            ViewBag.UserID = this.UserID.Value;
            return View(factures.Select(f => new FactureModel(f)).OrderByDescending(f => f.Date.Year).ThenByDescending(f => f.Date.Month).ThenByDescending(f => f.Date.Day).ToList());
        }


        [ValidateInput(false)]
        public ActionResult GridViewListeFacturePartial(int? idClient, DateTime dateDebut, DateTime dateFin)
        {
            SearchPieceVenteWithPiecesVenteModel model = new SearchPieceVenteWithPiecesVenteModel();
            model.Search = new SearchPieceVenteModel()
            {
                ClientID = idClient,
                DateDebut = dateDebut,
                DateFin = dateFin
            };
            model.PiecesVente = CEDAlfaiaAppWebService.GetService().GarageService.Factures().Where(d => d.DateFacture >= model.Search.DateDebut && d.DateFacture <= model.Search.DateFin && d.ClientID == (model.Search.ClientID != null ? model.Search.ClientID : d.ClientID));
            return PartialView("_GridViewListeFacturePartial", model);
        }

        [HttpGet]
        public ActionResult ListeAnnulees()
        {
            var factures = CEDAlfaiaAppWebService.GetService().GarageService.GetFacturesAnnulees();
            return View(factures.Select(f => new FactureModel(f)).OrderByDescending(f => f.Date.Year).ThenByDescending(f => f.Date.Month).ThenByDescending(f => f.Date.Day).ToList());
        }

        [HttpGet]
        public FileResult ImprimerParLot(string ids)
        {
            List<int> listIds = new JavaScriptSerializer().Deserialize<List<int>>(ids);

            List<PieceVente> factures = new List<PieceVente>();
            if (listIds.Any())
            {
                string nomFichier = "Factures " + DateTime.Now.ToShortDateString() + ".pdf";
                List<byte[]> listArray = new List<byte[]>();
                foreach (int id in listIds)
                {
                    PieceVente factureFound = CEDAlfaiaAppWebService.GetService().GarageService.GetFacture(id);
                    if (factureFound != null)
                        listArray.Add(editerFacture(factureFound));
                }
                return ExportDocument(Impression.JoinArrays(listArray), "pdf", nomFichier, true);
            }

            return null;
        }

        [HttpGet]
        public string EnvoyerComptable(string ids)
        {
            try
            {
                List<int> listIds = new JavaScriptSerializer().Deserialize<List<int>>(ids);

                List<PieceVente> factures = new List<PieceVente>();
                if (listIds.Any())
                {
                    string nomFichier = "Factures " + DateTime.Now.ToShortDateString() + ".pdf";
                    List<byte[]> listArray = new List<byte[]>();
                    List<PieceVente> pieces = new List<PieceVente>();
                    foreach (int id in listIds)
                    {
                        PieceVente factureFound = CEDAlfaiaAppWebService.GetService().GarageService.GetFacture(id);
                        if (factureFound != null)
                        {
                            listArray.Add(editerFacture(factureFound));
                            pieces.Add(factureFound);
                        }
                    }
                    //return File(Impression.JoinArrays(listArray), System.Net.Mime.MediaTypeNames.Application.Octet, nomFichier);
                    Stream pieceJointe = new MemoryStream(Impression.JoinArrays(listArray));

                    Parametrage parametrage = null;
                    if (this.UserID != null)
                        parametrage = CEDAlfaiaAppWebService.GetService().ParametrageService.GetParametrageUtilisateur(this.UserID.Value);

                    if (!string.IsNullOrEmpty(parametrage.ServeurSMTP) && parametrage.PortSMTP != null)
                    {
                        SmtpClient smtpClient = new SmtpClient(parametrage.ServeurSMTP, parametrage.PortSMTP.Value);
                        //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                        MailMessage message = new MailMessage();
                        message.Attachments.Add(new Attachment(pieceJointe, "Factures " + parametrage.NomEntreprise + ".pdf"));
                        message.Body = "Bonjour, \nci-joint les factures : \n";
                        foreach (var piece in pieces)
                        {
                            message.Body += " - " + piece.NumFacture + "\n";
                        }
                        message.Body += "\n\nCordialement,\n" + parametrage.NomEntreprise;
                        message.CC.Add(parametrage.EmailEntreprise);
                        message.To.Add(parametrage.EmailComptable);
                        message.Subject = "[" + parametrage.NomEntreprise + "] Factures " + DateTime.Now.ToShortDateString();
                        message.Priority = MailPriority.High;
                        message.From = new MailAddress(parametrage.EmailEnvoiSMTP);

                        smtpClient.EnableSsl = parametrage.IsSSL;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new System.Net.NetworkCredential(parametrage.EmailSMTP, parametrage.PasswordSMTP);

                        smtpClient.Send(message);

                        CEDAlfaiaAppWebService.GetService().GarageService.SetFacturesEnvoyes(pieces.Select(p => p.ID).ToList());

                        return "OK";
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public ActionResult Saisir(string identifiant, string typeId = null)
        {
            int id = 0;
            int.TryParse(identifiant, out id);

            int? idClient = null;
            int? idFacture = null;
            if (typeId == null)
                typeId = "0";
            switch (typeId)
            {
                // facture
                case "0":
                    idFacture = id;
                    break;
                // client
                case "1":
                    idClient = id;
                    break;
            }

            if (idFacture != null && idFacture > 0)
            {
                PieceVente facture = CEDAlfaiaAppWebService.GetService().GarageService.GetFacture(id);
                return View(new SaisirFactureModel(facture));
            }
            float montantMainDOeuvre = 0;
            if (this.UserID != null)
            {
                Parametrage parametrage = CEDAlfaiaAppWebService.GetService().ParametrageService.GetParametrageUtilisateur(this.UserID.Value);
                montantMainDOeuvre = parametrage.MainDOeuvreMontantHoraire;
            }
            return View(new SaisirFactureModel()
            {
                DureeMainDOeuvre = 1,
                MainDOeuvre = montantMainDOeuvre
            });
        }

        [HttpPost]
        public ActionResult Saisir(SaisirFactureModel pFacture)
        {
            if (!ModelState.IsValid)
            {
                return View(pFacture);
            }

            if (pFacture.Date == null)
            {
                ModelState.AddModelError("", "La date de la facture est obligatoire !");
                return View(pFacture);
            }

            try
            {
                return saisirFacture(pFacture);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Le facture n'a pas pu être enregistrée : " + ex.Message);
                return View(pFacture);
            }
        }

        private ActionResult saisirFacture(SaisirFactureModel pFacture)
        {
            if (pFacture != null)
            {
                float tauxTVA = 1;
                if (this.UtilisateurEnCours.Entreprise != null && this.UtilisateurEnCours.Entreprise.Parametrage != null)
                    tauxTVA = this.UtilisateurEnCours.Entreprise.Parametrage.TVA;

                int? idRdv = null;
                if (pFacture.RendezVous != null && !string.IsNullOrEmpty(pFacture.RendezVous.Sujet))
                {
                    RendezVous rdv = new RendezVous();
                    DateTime dateHeureDebut = DateTime.Parse(pFacture.RendezVous.DateDebut);
                    if (pFacture.RendezVous.HeureDebut.Split(':').Count() > 1)
                    {
                        int hours = int.Parse(pFacture.RendezVous.HeureDebut.Split(':').First());
                        int minutes = int.Parse(pFacture.RendezVous.HeureDebut.Split(':')[1]);
                        dateHeureDebut.AddHours(hours);
                        dateHeureDebut.AddMinutes(minutes);
                    }
                    DateTime dateHeureFin = DateTime.Parse(pFacture.RendezVous.DateFin);
                    if (pFacture.RendezVous.HeureFin.Split(':').Count() > 1)
                    {
                        int hours = int.Parse(pFacture.RendezVous.HeureFin.Split(':').First());
                        int minutes = int.Parse(pFacture.RendezVous.HeureFin.Split(':')[1]);
                        dateHeureFin.AddHours(hours);
                        dateHeureFin.AddMinutes(minutes);
                    }
                    rdv = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateRendezVous(pFacture.RendezVous.ID, pFacture.RendezVous.Sujet, pFacture.RendezVous.Duree, dateHeureDebut, dateHeureFin, pFacture.RendezVous.Commentaire);
                    idRdv = rdv.ID;
                }

                if (!pFacture.IsValide)
                {
                    // enregistrer facture
                    PieceVente facture = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateFacture(pFacture.IDFacture, pFacture.IDVoiture, DateTime.Parse(pFacture.Date), pFacture.MainDOeuvre, pFacture.DureeMainDOeuvre, pFacture.Kilometrage, pFacture.Remise, idRdv);

                    // enregistrer lignes facture
                    foreach (var ligne in pFacture.Lignes)
                    {
                        if (!ligne.IsEmpty)
                            CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateLigneFacture(ligne.IDLigne, facture.ID, null, ligne.Libelle, ligne.PrixGarageHT, ligne.PrixGarageHT + (ligne.PrixGarageHT * tauxTVA), ligne.PrixGarageHT / (1 - (ligne.Remise / 100)), ligne.PrixGarageHT / (1 - (ligne.Remise / 100)) + (ligne.PrixGarageHT / (1 - (ligne.Remise / 100)) * tauxTVA), ligne.Quantite, ligne.Remise);
                    }

                    // enregistrer lignes services
                    foreach (var service in pFacture.Services)
                    {
                        if (!service.IsEmpty && !service.IsSupprime)
                            CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateServicePieceVente(service.IDService, pFacture.IDFacture, service.Libelle, service.PrixClientTTC / 1.2F, service.PrixClientTTC, service.Quantite);
                        else if (!service.IsEmpty && service.IDService > 0)
                            CEDAlfaiaAppWebService.GetService().GarageService.DeleteServicePieceVente(service.IDService);
                    }
                }


                // enregistrer Devis Fournisseur
                byte[] devisFournisseur = null;
                string devisFournisseurFormatFichier = "";
                if (!string.IsNullOrEmpty(pFacture.DevisFornisseurBase64) && pFacture.DevisFornisseurBase64.Contains(','))
                {
                    devisFournisseurFormatFichier = pFacture.DevisFornisseurBase64.Split(',').First();
                    devisFournisseur = Convert.FromBase64String(pFacture.DevisFornisseurBase64.Split(',').Last());

                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(pFacture.IDFacture, TypeDocument.DevisFournisseur, "Devis fournisseur", devisFournisseur, devisFournisseurFormatFichier);
                }

                // enregistrer BL Fournisseur
                byte[] blFournisseur = null;
                string blFournisseurFormatFichier = "";
                if (!string.IsNullOrEmpty(pFacture.BonLivraisonFornisseurBase64) && pFacture.BonLivraisonFornisseurBase64.Contains(','))
                {
                    blFournisseurFormatFichier = pFacture.BonLivraisonFornisseurBase64.Split(',').First();
                    blFournisseur = Convert.FromBase64String(pFacture.BonLivraisonFornisseurBase64.Split(',').Last());

                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(pFacture.IDFacture, TypeDocument.BonLivraisonFournisseur, "Bon livraison fournisseur", blFournisseur, blFournisseurFormatFichier);
                }

                // enregistrer paiements
                /*foreach (var paiement in pFacture.Paiements)
                {
                    if (!paiement.IsEmpty && !paiement.IsSupprime)
                    {
                        if (paiement.IDPaiement != 0 || paiement.Montant > 0)
                            CEDAlfaiaAppWebService.GetService().GarageService.AddUpdatePieceVentePaiement(paiement.IDPaiement, pFacture.IDFacture, paiement.Date, paiement.Montant, paiement.TypePaiement);
                    }
                    else if (!paiement.IsEmpty && paiement.IDPaiement > 0)
                        CEDAlfaiaAppWebService.GetService().GarageService.DeletePaiementPieceVente(paiement.IDPaiement);
                }
                */
                CEDAlfaiaAppWebService.GetService().GarageService.UpdateTotauxPieceVente(pFacture.IDFacture);

                return RedirectToAction("Liste");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Annuler(int? idFacture)
        {
            if (idFacture == null)
            {
                ModelState.AddModelError("", "La facture à annuler est obligatoire.");
                return View();
            }
            PieceVente facture = BLL.CEDAlfaiaAppWebService.GetService().GarageService.GetFacture(idFacture.Value);
            if (facture == null)
            {
                ModelState.AddModelError("", "La facture à annuler est obligatoire.");
                return View();
            }
            return View(new AnnulerFactureModel()
            {
                IDFacture = idFacture.Value,
                Numero = facture.NumFacture
            });
        }

        [HttpPost]
        public ActionResult Annuler(AnnulerFactureModel pModel)
        {
            if (!ModelState.IsValid)
            {
                return View(pModel);
            }

            if (string.IsNullOrEmpty(pModel.CommentaireAnnulation))
            {
                ModelState.AddModelError("", "Le motif d'annulation est obligatoire !");
                return View(pModel);
            }

            try
            {
                CEDAlfaiaAppWebService.GetService().GarageService.AnnulerFacture(pModel.IDFacture, pModel.CommentaireAnnulation);
                return RedirectToAction("Liste");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Le facture n'a pas pu être annulé : " + ex.Message);
                return View(pModel);
            }
        }

        [HttpPost]
        public string RepasserEnFacture(string idFacture)
        {
            try
            {
                int id = 0;
                int.TryParse(idFacture, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.RepasserEnFacture(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public FileResult Editer(int idFacture)
        {
            PieceVente factureFound = CEDAlfaiaAppWebService.GetService().GarageService.GetFacture(idFacture);
            if (factureFound != null)
            {
                return ExportDocument(editerFacture(factureFound), "pdf", "Facture " + factureFound.NumFacture.Value + ".pdf", true);
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

        public byte[] editerFacture(PieceVente facture)
        {
            /*
            byte[] fichier = null;
            Parametrage parametrage = null;
            if (this.UserID != null)
                parametrage = CEDAlfaiaAppWebService.GetService().ParametrageService.GetParametrageUtilisateur(this.UserID.Value);

            if (parametrage != null && parametrage.IsSavePieceDeVente && facture.Documents.Any(d => d.TypeDocument == TypeDocument.Facture))
                fichier = facture.Documents.Where(d => d.TypeDocument == TypeDocument.Facture).First().Doc;

            if (fichier == null)
            {
                string cheminFichier = _createPDF(facture, parametrage);
                fichier = System.IO.File.ReadAllBytes(cheminFichier);
                if (parametrage != null && parametrage.IsSavePieceDeVente)
                {
                    DocumentPieceVente docPV = new DocumentPieceVente(facture.ID, "Facture " + facture.NumFacture.Value, TypeDocument.Facture, fichier, null);
                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(docPV);
                }
            }

            return fichier;*/

            XtraReport report = new FactureXtraReport();
            using (MemoryStream ms = new MemoryStream())
            {
                var dataSource = report.DataSource as EFDataSource;
                dataSource.Filters["PiecesVente"] = "ID = " + facture.ID;
                report.DataSource = dataSource;
                report.ExportToPdf(ms);
                return ms.ToArray();
            }
        }

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
        /*
        [HttpPost]
        public string ValiderFacture(string idFacture)
        {
            try
            {
                int id = 0;
                int.TryParse(idFacture, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.ValiderFacture(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        */
        [HttpGet]
        public ActionResult ValiderFacture(string idFacture)
        {
            try
            {
                int id = 0;
                int.TryParse(idFacture, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.ValiderFacture(id);
                return RedirectToAction("Liste");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Liste");
            }
        }
        [HttpPost]
        public string PayerFacture(string idFacture, string mode)
        {
            try
            {
                int id = 0;
                int.TryParse(idFacture, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.PayerFacture(id, (TypePaiement)Convert.ToInt32(mode));
                return "OK";
            }
            catch (Exception ex)
            {
                return "KO";
            }
        }
        #region Impression
        private string _createPDF(PieceVente pFacture, Parametrage pParametrage)
        {
            Document document = new Document(PageSize.A4);
            string appRootDir = ControllerContext.HttpContext.Server.MapPath("~");
            try
            {
                var nomFacture = pFacture.Client.Nom + " " + pFacture.NumFacture;
                var dossierFichier = "Factures";
                var filePath = appRootDir + "\\" + dossierFichier + "\\" + nomFacture + ".pdf";
                using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (Document doc = new Document(PageSize.A4, 36, 36, 36, 36))
                using (PdfWriter writer = PdfWriter.GetInstance(doc, fs))
                {
                    writer.PageEvent = new Impression.PDFFooter(pFacture, pParametrage);

                    doc.Open();

                    doc.Add(Impression.GetEntete(pFacture));

                    doc.Add(Impression.GetTableauReparations(pFacture));

                    List<Rectangle> rectangles = Impression.GetFooter(pFacture);
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
                    if (pFacture.NumFacture != null)
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
        #endregion

        [HttpGet]
        public async Task<ActionResult> LigneFactureChange(string idLigne, string libelle, string quantite, string remise, string prixGarageHT, string indexLigne, string isEmpty, string isSupprime)
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

            var model = new SaisirLignesFactureModel(id, libelle, quantiteSaisie, remiseSaisie, prixHTSaisie, prixHTSaisie + (prixHTSaisie * tauxTVA), prixHTSaisie / (1 - (remiseSaisie / 100)), prixHTSaisie / (1 - (remiseSaisie / 100)) + (prixHTSaisie / (1 - (remiseSaisie / 100)) * tauxTVA), indexLigneSaisie, isSupprimeSaisie, isEmptySaisie);

            return PartialView("_LigneFacture", model);
        }
        [HttpGet]
        public async Task<ActionResult> LigneFactureAdd(string indexLigne)
        {
            int indexLigneSaisie = int.Parse(indexLigne);

            var model = new SaisirLignesFactureModel();
            model.IndexLigne = indexLigneSaisie;

            return PartialView("_LigneFacture", model);
        }
        [HttpGet]
        public async Task<ActionResult> PaiementAdd(string indexLigne)
        {
            int indexLigneSaisie = int.Parse(indexLigne);

            var model = new SaisirPaiementPieceVenteModel();
            //model.IndexLigne = indexLigneSaisie;

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