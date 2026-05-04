using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.DAL.Parametrage;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaAppV2.Models;
using CEDAlfaiaAppV2.Reports;
using DevExpress.DataAccess.EntityFramework;
using DevExpress.XtraReports.UI;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace CEDAlfaiaAppV2.Controllers
{
    public class PiecesVenteController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Liste(string idClient, DateTime? dateDebut, DateTime? dateFin, string typeListe = null)
        {
            int? client = null;
            if (!string.IsNullOrEmpty(idClient))
                client = Convert.ToInt32(idClient);

            if (dateDebut == null)
                dateDebut = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            if (dateFin == null)
                dateFin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));

            ViewBag.UserID = this.UserID.Value;

            if (typeListe == null)
                typeListe = "Factures";

            return View(new SearchPieceVenteModel()
            {
                ClientID = client,
                DateDebut = dateDebut.Value,
                DateFin = dateFin.Value,
                TypeListe = typeListe
            });
        }

        [ValidateInput(false)]
        public ActionResult GridViewListePiecesVentePartial(int? idClient, DateTime dateDebut, DateTime dateFin, string typeListe)
        {
            SearchPieceVenteWithPiecesVenteModel model = new SearchPieceVenteWithPiecesVenteModel();
            model.Search = new SearchPieceVenteModel()
            {
                ClientID = idClient,
                DateDebut = dateDebut,
                DateFin = dateFin,
                TypeListe = typeListe
            };

            switch (typeListe)
            {
                case "Factures":
                    model.PiecesVente = CEDAlfaiaAppWebService.GetService().GarageService.Factures().Where(d => d.DateFacture >= model.Search.DateDebut && d.DateFacture <= model.Search.DateFin && d.ClientID == (model.Search.ClientID != null ? model.Search.ClientID : d.ClientID) && !d.IsFactureAnnule);
                    break;
                case "Devis":
                    model.PiecesVente = CEDAlfaiaAppWebService.GetService().GarageService.Devis().Where(d => d.DateFacture == null && d.DateDevis >= model.Search.DateDebut && d.DateDevis <= model.Search.DateFin && d.ClientID == (model.Search.ClientID != null ? model.Search.ClientID : d.ClientID));
                    break;
                case "FacturesAnnulees":
                    model.PiecesVente = CEDAlfaiaAppWebService.GetService().GarageService.GetFacturesAnnulees().Where(d => d.DateFacture >= model.Search.DateDebut && d.DateFacture <= model.Search.DateFin && d.ClientID == (model.Search.ClientID != null ? model.Search.ClientID : d.ClientID));
                    break;
                case "FacturesAEnvoyer":
                    model.PiecesVente = CEDAlfaiaAppWebService.GetService().GarageService.Factures().Where(d => d.DateFacture >= model.Search.DateDebut && d.DateFacture <= model.Search.DateFin && d.ClientID == (model.Search.ClientID != null ? model.Search.ClientID : d.ClientID) && !d.IsFactureAnnule && d.IsValide);
                    break;
            }
            return PartialView("_GridViewListePiecesVentePartial", model);
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

        [HttpPost]
        public string EnvoyerComptable(string ids)
        {
            try
            {
                List<int> listIds = new JavaScriptSerializer().Deserialize<List<int>>(ids);
                List<FichierComptable> fichier = new List<FichierComptable>();

                if (listIds.Any())
                {
                    string nomFichier = "Factures " + DateTime.Now.ToShortDateString() + ".pdf";
                    List<byte[]> listArray = new List<byte[]>();
                    List<PieceVente> pieces = new List<PieceVente>();
                    foreach (int id in listIds.OrderBy(t => t))
                    {
                        PieceVente factureFound = CEDAlfaiaAppWebService.GetService().GarageService.GetFacture(id);
                        if (factureFound != null)
                        {
                            listArray.Add(editerFacture(factureFound));
                            pieces.Add(factureFound);

                            float mainDOeuvreHT = (factureFound.MainDOeuvreMontantHoraire * factureFound.MainDOeuvreDuree) /*/ 1.2F*/;
                            fichier.Add(new FichierComptable(factureFound.DateFacture.Value.ToShortDateString(), "", "C" + factureFound.Client.Code, "", ((string.IsNullOrEmpty(factureFound.Client.Prenom) ? "" : factureFound.Client.Prenom).Trim() + " " + (string.IsNullOrEmpty(factureFound.Client.Nom) ? "" : factureFound.Client.Nom)).Trim(), factureFound.TotalTTC.ToString("0.00"), ""));
                            fichier.Add(new FichierComptable(factureFound.DateFacture.Value.ToShortDateString(), factureFound.NumFacture.ToString(), "706100", "TN", ((string.IsNullOrEmpty(factureFound.Client.Prenom) ? "" : factureFound.Client.Prenom).Trim() + " " + (string.IsNullOrEmpty(factureFound.Client.Nom) ? "" : factureFound.Client.Nom)).Trim(), "", mainDOeuvreHT.ToString("0.00")));
                            fichier.Add(new FichierComptable(factureFound.DateFacture.Value.ToShortDateString(), factureFound.NumFacture.ToString(), "707100", "TN", ((string.IsNullOrEmpty(factureFound.Client.Prenom) ? "" : factureFound.Client.Prenom).Trim() + " " + (string.IsNullOrEmpty(factureFound.Client.Nom) ? "" : factureFound.Client.Nom)).Trim(), "", (factureFound.TotalHT - mainDOeuvreHT).ToString("0.00")));
                            fichier.Add(new FichierComptable(factureFound.DateFacture.Value.ToShortDateString(), factureFound.NumFacture.ToString(), "445710", "", ((string.IsNullOrEmpty(factureFound.Client.Prenom) ? "" : factureFound.Client.Prenom).Trim() + " " + (string.IsNullOrEmpty(factureFound.Client.Nom) ? "" : factureFound.Client.Nom)).Trim(), "", factureFound.MontantTVA.ToString("0.00")));
                        }
                    }

                    var csv = CsvSerializer.SerializeToCsv(fichier);
                    System.IO.File.WriteAllText(HttpRuntime.AppDomainAppPath + "import.csv", csv);

                    //return File(Impression.JoinArrays(listArray), System.Net.Mime.MediaTypeNames.Application.Octet, nomFichier);
                    Stream pieceJointe = new MemoryStream(Impression.JoinArrays(listArray));
                    Stream pjImport = new FileStream(HttpRuntime.AppDomainAppPath + "import.csv", FileMode.Open);

                    Parametrage parametrage = null;
                    if (this.UserID != null)
                        parametrage = CEDAlfaiaAppWebService.GetService().ParametrageService.GetParametrageUtilisateur(this.UserID.Value);

                    if (!string.IsNullOrEmpty(parametrage.ServeurSMTP) && parametrage.PortSMTP != null)
                    {
                        SmtpClient smtpClient = new SmtpClient(parametrage.ServeurSMTP, parametrage.PortSMTP.Value);
                        //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                        MailMessage message = new MailMessage();
                        message.Attachments.Add(new Attachment(pieceJointe, "Factures " + parametrage.NomEntreprise + ".pdf"));
                        message.Attachments.Add(new Attachment(pjImport, "import.csv"));
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
        public ActionResult Saisir(string identifiant, string typeId = null, string typePieceVente = null)
        {
            int id = 0;
            int.TryParse(identifiant, out id);

            int? idClient = null;
            int? idPieceVente = null;
            if (typeId == null)
                typeId = "0";
            switch (typeId)
            {
                // facture
                case "0":
                    idPieceVente = id;
                    break;
                // client
                case "1":
                    idClient = id;
                    break;
            }

            if (typePieceVente == null)
                typePieceVente = "Facture";

            if (idPieceVente != null && idPieceVente > 0)
            {
                PieceVente pieceVente = CEDAlfaiaAppWebService.GetService().GarageService.GetPieceVente(id);
                return View(new SaisirPieceVenteModel(pieceVente));
            }
            float montantMainDOeuvre = 0;
            if (this.UserID != null)
            {
                Parametrage parametrage = CEDAlfaiaAppWebService.GetService().ParametrageService.GetParametrageUtilisateur(this.UserID.Value);
                montantMainDOeuvre = parametrage.MainDOeuvreMontantHoraire;
            }
            return View(new SaisirPieceVenteModel(typePieceVente)
            {
                DureeMainDOeuvre = 1,
                MainDOeuvre = montantMainDOeuvre
            });
        }

        [HttpPost]
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
        }

        [HttpPost]
        public ActionResult Saisir(SaisirPieceVenteModel pPieceVente)
        {
            if (!ModelState.IsValid)
            {
                return View(pPieceVente);
            }

            if (pPieceVente.Date == null)
            {
                ModelState.AddModelError("", "La date de la facture est obligatoire !");
                return View(pPieceVente);
            }
            if (pPieceVente.TypePiece == "Facture" && pPieceVente.Kilometrage == 0)
            {
                ModelState.AddModelError("", "Le kilometrage est obligatoire !");
                return View(pPieceVente);
            }

            try
            {
                return saisirPieceVente(pPieceVente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "La pièce de vente n'a pas pu être enregistrée : " + ex.Message);
                return View(pPieceVente);
            }
        }

        private ActionResult saisirPieceVente(SaisirPieceVenteModel pPieceVente)
        {
            if (pPieceVente != null)
            {
                float tauxTVA = 1;
                if (this.UtilisateurEnCours.Entreprise != null && this.UtilisateurEnCours.Entreprise.Parametrage != null)
                    tauxTVA = this.UtilisateurEnCours.Entreprise.Parametrage.TVA;

                int? idRdv = null;
                if (pPieceVente.RendezVous != null && !string.IsNullOrEmpty(pPieceVente.RendezVous.Sujet))
                {
                    RendezVous rdv = new RendezVous();
                    rdv = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateRendezVous(pPieceVente.RendezVous.ID, pPieceVente.RendezVous.Sujet, pPieceVente.RendezVous.Duree, pPieceVente.RendezVous.Debut.Value, pPieceVente.RendezVous.Fin.Value, pPieceVente.RendezVous.Commentaire);
                    idRdv = rdv.ID;
                }

                CEDAlfaiaAppWebService.GetService().GarageService.DeletePaiementsPieceVente(pPieceVente.IDPieceVente);

                if (pPieceVente.TypePiece == "Facture")
                {
                    PieceVente facture = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateFacture(pPieceVente.IDPieceVente, pPieceVente.IDVoiture.Value, pPieceVente.Date, pPieceVente.MainDOeuvre, pPieceVente.DureeMainDOeuvre, pPieceVente.Kilometrage, pPieceVente.Remise, idRdv);

                    if (!pPieceVente.IsValide)
                    {
                        // enregistrer facture

                        List<int> idsLignesFacture = facture.Lignes.Select(s => s.ID).ToList();
                        List<int> idsLignesNewFacture = pPieceVente.Lignes.Where(s => s.IDLigne != 0).Select(s => s.IDLigne).ToList();
                        List<int> idsLignesASupprimer = idsLignesFacture.Except(idsLignesNewFacture).ToList();

                        // enregistrer lignes facture
                        foreach (var ligne in pPieceVente.Lignes)
                        {
                            float prixGarageHT = float.Parse(ligne.PrixGarageHT.Replace('.', ','));
                            float prixGarageTTC = float.Parse(ligne.PrixGarageTTC.Replace('.', ','));
                            float prixClientHT = float.Parse(ligne.PrixClientHT.Replace('.', ','));
                            float prixClientTTC = float.Parse(ligne.PrixClientTTC.Replace('.', ','));
                            CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateLigneFacture(ligne.IDLigne, facture.ID, null, ligne.Libelle, prixGarageHT, prixGarageTTC, prixClientHT, prixClientTTC, ligne.Quantite, ligne.Remise);
                        }

                        foreach (var idLigneASupprimer in idsLignesASupprimer)
                        {
                            CEDAlfaiaAppWebService.GetService().GarageService.DeleteLigneFacture(idLigneASupprimer);
                        }
                    }

                    pPieceVente.IDPieceVente = facture.ID;
                }
                else
                {
                    // enregistrer devis
                    PieceVente devis = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDevis(pPieceVente.IDPieceVente, pPieceVente.IDVoiture.Value, pPieceVente.Date, pPieceVente.MainDOeuvre, pPieceVente.DureeMainDOeuvre, pPieceVente.Kilometrage, pPieceVente.Remise, idRdv, pPieceVente.IsDevisEnvoye);

                    List<int> idsLignesDevis = devis.Lignes.Select(s => s.ID).ToList();
                    List<int> idsLignesNewDevis = pPieceVente.Lignes.Where(s => s.IDLigne != 0).Select(s => s.IDLigne).ToList();
                    List<int> idsLignesASupprimer = idsLignesDevis.Except(idsLignesNewDevis).ToList();

                    // enregistrer lignes facture
                    foreach (var ligne in pPieceVente.Lignes)
                    {
                        float prixGarageHT = float.Parse(ligne.PrixGarageHT.Replace('.', ','));
                        float prixGarageTTC = float.Parse(ligne.PrixGarageTTC.Replace('.', ','));
                        float prixClientHT = float.Parse(ligne.PrixClientHT.Replace('.', ','));
                        float prixClientTTC = float.Parse(ligne.PrixClientTTC.Replace('.', ','));
                        CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateLigneFacture(ligne.IDLigne, devis.ID, null, ligne.Libelle, prixGarageHT, prixGarageTTC, prixClientHT, prixClientTTC, ligne.Quantite, ligne.Remise);
                    }

                    foreach (var idLigneASupprimer in idsLignesASupprimer)
                    {
                        CEDAlfaiaAppWebService.GetService().GarageService.DeleteLigneDevis(idLigneASupprimer);
                    }

                    pPieceVente.IDPieceVente = devis.ID;
                }

                // enregistrer Devis Fournisseur
                byte[] devisFournisseur = null;
                string devisFournisseurFormatFichier = "";
                if (!string.IsNullOrEmpty(pPieceVente.DevisFornisseurBase64) && pPieceVente.DevisFornisseurBase64.Contains(','))
                {
                    devisFournisseurFormatFichier = pPieceVente.DevisFornisseurBase64.Split(',').First();
                    devisFournisseur = Convert.FromBase64String(pPieceVente.DevisFornisseurBase64.Split(',').Last());

                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(pPieceVente.IDPieceVente, TypeDocument.DevisFournisseur, "Devis fournisseur", devisFournisseur, devisFournisseurFormatFichier);
                }

                // enregistrer BL Fournisseur
                byte[] blFournisseur = null;
                string blFournisseurFormatFichier = "";
                if (!string.IsNullOrEmpty(pPieceVente.BonLivraisonFornisseurBase64) && pPieceVente.BonLivraisonFornisseurBase64.Contains(','))
                {
                    blFournisseurFormatFichier = pPieceVente.BonLivraisonFornisseurBase64.Split(',').First();
                    blFournisseur = Convert.FromBase64String(pPieceVente.BonLivraisonFornisseurBase64.Split(',').Last());

                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateDocument(pPieceVente.IDPieceVente, TypeDocument.BonLivraisonFournisseur, "Bon livraison fournisseur", blFournisseur, blFournisseurFormatFichier);
                }

                // enregistrer lignes facture
                foreach (var paiement in pPieceVente.Paiements)
                {
                    float montant = 0;
                    float.TryParse(paiement.Montant.Replace('.', ','), out montant);
                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdatePieceVentePaiement(paiement.IDPaiement, pPieceVente.IDPieceVente, paiement.Date, montant, paiement.TypePaiement);
                }

                CEDAlfaiaAppWebService.GetService().GarageService.UpdateTotauxPieceVente(pPieceVente.IDPieceVente);
                CEDAlfaiaAppWebService.GetService().GarageService.PasserProspectEnClientByVoiture(pPieceVente.IDVoiture.Value);

                return RedirectToAction("Liste", new { typeListe = pPieceVente.TypePiece == "Facture" ? "Factures" : "Devis" });
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
            PieceVente facture = CEDAlfaiaAppWebService.GetService().GarageService.GetFacture(idFacture.Value);
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
        public FileResult Editer(int idPieceVente)
        {
            PieceVente pieceVenteFound = CEDAlfaiaAppWebService.GetService().GarageService.GetPieceVente(idPieceVente);
            if (pieceVenteFound != null)
            {
                if (pieceVenteFound.DateFacture != null)
                    return ExportDocument(editerFacture(pieceVenteFound), "pdf", "Facture " + pieceVenteFound.NumFacture.Value + ".pdf", true);
                else
                    return ExportDocument(editerDevis(pieceVenteFound), "pdf", "Facture " + pieceVenteFound.NumDevis.Value + ".pdf", true);

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

        public byte[] editerDevis(PieceVente devis)
        {
            XtraReport report = new DevisXtraReport();
            using (MemoryStream ms = new MemoryStream())
            {
                var dataSource = report.DataSource as EFDataSource;
                dataSource.Filters["PiecesVente"] = "ID = " + devis.ID;
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

        [HttpPost]
        public string Payer(string idPieceVente, string mode)
        {
            try
            {
                int id = 0;
                int.TryParse(idPieceVente, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.PayerPieceVente(id, (TypePaiement)Convert.ToInt32(mode));
                return "OK";
            }
            catch (Exception ex)
            {
                return "KO";
            }
        }
    }
}