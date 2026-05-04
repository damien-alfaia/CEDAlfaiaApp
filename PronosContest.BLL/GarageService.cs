using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Garage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CEDAlfaiaApp.BLL
{
    public class GarageService
    {
        private CEDAlfaiaAppContext _CEDAlfaiaAppContextDatabase;

        internal GarageService(CEDAlfaiaAppContext pCEDAlfaiaAppContextDatabase)
        {
            _CEDAlfaiaAppContextDatabase = pCEDAlfaiaAppContextDatabase;
        }

        /*
        public void init(List<Client> clients, List<Marque> marques, List<Modele> modeles, List<Voiture> voitures, List<Piece> devis, List<Facture> factures, List<DevisLigne> lignes)
        {
            //foreach (var dl in _CEDAlfaiaAppContextDatabase.DevisLignes) _CEDAlfaiaAppContextDatabase.DevisLignes.Remove(dl);
            //foreach (var f in _CEDAlfaiaAppContextDatabase.Factures) _CEDAlfaiaAppContextDatabase.Factures.Remove(f);
            //foreach (var d in _CEDAlfaiaAppContextDatabase.Devis) _CEDAlfaiaAppContextDatabase.Devis.Remove(d);
            foreach (var v in _CEDAlfaiaAppContextDatabase.Voitures) _CEDAlfaiaAppContextDatabase.Voitures.Remove(v);
            foreach (var m in _CEDAlfaiaAppContextDatabase.Modeles) _CEDAlfaiaAppContextDatabase.Modeles.Remove(m);
            foreach (var m in _CEDAlfaiaAppContextDatabase.Marques) _CEDAlfaiaAppContextDatabase.Marques.Remove(m);
            foreach (var c in _CEDAlfaiaAppContextDatabase.Clients) _CEDAlfaiaAppContextDatabase.Clients.Remove(c);

            _CEDAlfaiaAppContextDatabase.Clients.AddRange(clients);
            _CEDAlfaiaAppContextDatabase.Marques.AddRange(marques);
            _CEDAlfaiaAppContextDatabase.Modeles.AddRange(modeles);
            _CEDAlfaiaAppContextDatabase.Voitures.AddRange(voitures);
            _CEDAlfaiaAppContextDatabase.Devis.AddRange(devis);
            _CEDAlfaiaAppContextDatabase.Factures.AddRange(factures);
            _CEDAlfaiaAppContextDatabase.DevisLignes.AddRange(lignes);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        */

        #region Clients
        public IQueryable<Client> Clients()
        {
            return _CEDAlfaiaAppContextDatabase.Clients.Where(c => !c.IsProspect);
        }
        public IQueryable<Client> Prospects()
        {
            return _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.IsProspect);
        }
        public List<Client> GetClients()
        {
            return _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.DateSuppression == null).ToList();
        }
        public List<Client> GetClientsSupprimes()
        {
            return _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.DateSuppression != null).ToList();
        }
        public Client GetClient(int id)
        {
            return _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.ID == id).FirstOrDefault();
        }
        public Client AddUpdateClient(int pID, string pNom, string pPrenom, string pTelephone, string pEmail, string pLigne1, string pLigne2, string pLigne3, string pCodePostal, string pVille, string pPays, string pComplements, float? pRemise, bool pIsProspect)
        {
            Client clientResult = new Client();
            if (pID > 0)
                clientResult = _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.ID == pID).FirstOrDefault();

            clientResult.Nom = pNom;
            clientResult.Prenom = pPrenom;
            clientResult.Telephone = pTelephone;
            clientResult.Email = pEmail;
            if (clientResult.Adresse == null)
                clientResult.Adresse = new DAL.Shared.Adresse();
            clientResult.Adresse.Ligne1 = pLigne1;
            clientResult.Adresse.Ligne2 = pLigne2;
            clientResult.Adresse.Ligne3 = pLigne3;
            clientResult.Adresse.CodePostal = pCodePostal;
            clientResult.Adresse.Ville = pVille;
            clientResult.Adresse.Pays = pPays;
            clientResult.Remise = pRemise;
            clientResult.IsProspect = pIsProspect;
            if (pID == 0)
                clientResult = _CEDAlfaiaAppContextDatabase.Clients.Add(clientResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();

            GenererCodeClient(clientResult.ID);
            return clientResult;
        }
        public Voiture GetVoiture(int pID)
        {
            return _CEDAlfaiaAppContextDatabase.Voitures.Where(v => v.ID == pID).FirstOrDefault();
        }
        public Voiture AddUpdateVoitureClient(int pIDModele, string pImmatriculation, int pIDClient, int? pIDVoiture = null)
        {
            try
            {
                Voiture voitureResult = new Voiture();
                if (pIDVoiture != null)
                    voitureResult = GetVoiture(pIDVoiture.Value);

                Modele modeleFound = this.GetModeleByID(pIDModele);
                if (modeleFound == null)
                    throw new Exception("Le modèle de la voiture est obligatoire.");

                if (string.IsNullOrEmpty(pImmatriculation))
                    throw new Exception("L'immatriculation de la voiture est obligatoire.");

                voitureResult.ModeleID = modeleFound.ID;
                voitureResult.Immatriculation = pImmatriculation;
                voitureResult.ClientID = pIDClient;

                if (pIDVoiture == null)
                    _CEDAlfaiaAppContextDatabase.Voitures.Add(voitureResult);
                _CEDAlfaiaAppContextDatabase.SaveChanges();

                GenererCodeClient(voitureResult.ID);

                return voitureResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteVoitureClient(int pIDVoiture)
        {
            try
            {
                Voiture voitureFound = _CEDAlfaiaAppContextDatabase.Voitures.Where(v => v.ID == pIDVoiture).FirstOrDefault();
                if (voitureFound == null)
                    throw new Exception("La voiture est obligatoire.");

                voitureFound.DateSuppression = DateTime.Now;

                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void DeleteClient(int pID)
        {
            Client clientResult = new Client();
            if (pID > 0)
                clientResult = _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.ID == pID).FirstOrDefault();

            clientResult.DateSuppression = DateTime.Now;

            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public void PasserProspectEnClient(int pID)
        {
            try
            {
                Client prospectFound = _CEDAlfaiaAppContextDatabase.Clients.Where(v => v.ID == pID).FirstOrDefault();
                if (prospectFound == null)
                    throw new Exception("Le prospect est obligatoire.");

                prospectFound.IsProspect = false;

                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void PasserProspectEnClientByVoiture(int pIDVoiture)
        {
            try
            {
                Client prospectFound = _CEDAlfaiaAppContextDatabase.Clients.Where(v => v.Voitures.Any(voit => voit.ID == pIDVoiture)).FirstOrDefault();
                if (prospectFound == null)
                    throw new Exception("Le prospect est obligatoire.");

                prospectFound.IsProspect = false;

                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ReactiverClient(int pID)
        {
            Client clientResult = new Client();
            if (pID > 0)
                clientResult = _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.ID == pID).FirstOrDefault();

            clientResult.DateSuppression = null;

            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public void GenererProspects()
        {
            List<PieceVente> piecesVente = _CEDAlfaiaAppContextDatabase.PiecesVente.ToList();
            var clients = _CEDAlfaiaAppContextDatabase.Clients;
            foreach (var client in clients)
            {
                client.IsProspect = true;
                if (piecesVente.Where(p => p.ClientID == client.ID).Any())
                    client.IsProspect = false;
            }
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public void GenererCodesClients()
        {
            foreach (var client in _CEDAlfaiaAppContextDatabase.Clients)
            {
                client.Code = null;
            }
            _CEDAlfaiaAppContextDatabase.SaveChanges();
            List<Client> clientsModifies = new List<Client>();
            foreach (var client in _CEDAlfaiaAppContextDatabase.Clients.OrderBy(c => c.Nom).ToList())
            {
                if (client.Code == null)
                {
                    string codeClient = "";
                    string code3Lettres = "";
                    if (!string.IsNullOrEmpty(client.Nom))
                        code3Lettres = client.Nom.Replace(" ", "");
                    if (code3Lettres.Length < 3 && !string.IsNullOrEmpty(client.Prenom))
                        code3Lettres += client.Prenom.Replace(" ", "");
                    if (code3Lettres.Length < 3)
                        code3Lettres += "###";
                    code3Lettres = code3Lettres.ToUpper().Substring(0, 3);
                    codeClient = code3Lettres;

                    int nbMemeDebut = clientsModifies.Count(c => c.Code != null && c.Code.StartsWith(code3Lettres));
                    codeClient += (nbMemeDebut + 1).ToString("00");

                    client.Code = codeClient;

                    clientsModifies.Add(client);
                }
            }
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public void GenererCodeClient(int idClient)
        {
            var clientFound = _CEDAlfaiaAppContextDatabase.Clients.Where(c => c.ID == idClient).FirstOrDefault();
            if (clientFound != null && clientFound.Code == null)
            {
                string codeClient = "";
                string code3Lettres = "";
                if (!string.IsNullOrEmpty(clientFound.Nom))
                    code3Lettres = clientFound.Nom.Replace(" ", "");
                if (code3Lettres.Length < 3 && !string.IsNullOrEmpty(clientFound.Prenom))
                    code3Lettres += clientFound.Prenom.Replace(" ", "");
                if (code3Lettres.Length < 3)
                    code3Lettres += "###";
                code3Lettres = code3Lettres.ToUpper().Substring(0, 3);
                codeClient = code3Lettres;

                int nbMemeDebut = _CEDAlfaiaAppContextDatabase.Clients.Count(c => c.Code != null && c.Code.StartsWith(code3Lettres));
                codeClient += (nbMemeDebut + 1).ToString("00");

                clientFound.Code = codeClient;
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
        public IQueryable<Voiture> Voitures()
        {
            return _CEDAlfaiaAppContextDatabase.Voitures;
        }
        #endregion

        #region Marques
        public List<Marque> GetAllMarques()
        {
            return _CEDAlfaiaAppContextDatabase.Marques.ToList();
        }
        #endregion

        #region Modeles
        public IQueryable<Modele> Modeles()
        {
            return _CEDAlfaiaAppContextDatabase.Modeles;
        }
        public List<Modele> GetAllModeles()
        {
            return _CEDAlfaiaAppContextDatabase.Modeles.ToList();
        }
        public List<Modele> GetModelesByMarque(int idMarque)
        {
            return _CEDAlfaiaAppContextDatabase.Modeles.Where(m => m.MarqueID == idMarque).ToList();
        }
        public Modele GetModeleByID(int idModele)
        {
            return _CEDAlfaiaAppContextDatabase.Modeles.Where(m => m.ID == idModele).FirstOrDefault();
        }
        #endregion

        #region Fournisseurs
        public List<Fournisseur> GetFournisseurs()
        {
            return _CEDAlfaiaAppContextDatabase.Fournisseurs.ToList();
        }
        public Fournisseur GetFournisseur(int id)
        {
            return _CEDAlfaiaAppContextDatabase.Fournisseurs.Where(c => c.ID == id).FirstOrDefault();
        }
        public Fournisseur AddUpdateFournisseur(int pID, string pNom, string pLigne1, string pLigne2, string pLigne3, string pCodePostal, string pVille, string pPays, string pCommentaire)
        {
            Fournisseur fournisseurResult = new Fournisseur();
            if (pID > 0)
                fournisseurResult = _CEDAlfaiaAppContextDatabase.Fournisseurs.Where(c => c.ID == pID).FirstOrDefault();

            fournisseurResult.Nom = pNom;
            if (fournisseurResult.Adresse == null)
                fournisseurResult.Adresse = new DAL.Shared.Adresse();
            fournisseurResult.Adresse.Ligne1 = pLigne1;
            fournisseurResult.Adresse.Ligne2 = pLigne2;
            fournisseurResult.Adresse.Ligne3 = pLigne3;
            fournisseurResult.Adresse.CodePostal = pCodePostal;
            fournisseurResult.Adresse.Ville = pVille;
            fournisseurResult.Adresse.Pays = pPays;
            fournisseurResult.Commentaire = pCommentaire;

            if (pID == 0)
                fournisseurResult = _CEDAlfaiaAppContextDatabase.Fournisseurs.Add(fournisseurResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return fournisseurResult;
        }
        public void DeleteFournisseur(int pID)
        {
            Fournisseur fournisseurResult = new Fournisseur();
            if (pID > 0)
                fournisseurResult = _CEDAlfaiaAppContextDatabase.Fournisseurs.Where(c => c.ID == pID).FirstOrDefault();

            fournisseurResult.DateSuppression = DateTime.Now;

            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        #endregion

        #region Devis
        public IQueryable<PieceVente> Devis()
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(p => p.NumDevis != null && p.NumFacture == null);
        }
        public List<PieceVente> GetAllDevis()
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumDevis != null && pv.NumFacture == null).ToList();
        }
        public List<PieceVente> GetDevisBetweenDates(DateTime pDateDebut, DateTime pDateFin)
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumDevis != null && pv.NumFacture == null && pv.DateDevis >= pDateDebut && pv.DateDevis <= pDateFin).ToList();
        }
        public PieceVente GetDevis(int id)
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumDevis != null && pv.NumFacture == null && pv.ID == id).FirstOrDefault();
        }
        public void DeleteDevis(int pID)
        {
            PieceVente devisResult = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(c => c.ID == pID && c.NumFacture == null).FirstOrDefault();
            if (devisResult != null)
            {
                _CEDAlfaiaAppContextDatabase.PieceVentePaiements.RemoveRange(devisResult.Paiements);
                _CEDAlfaiaAppContextDatabase.Documents.RemoveRange(devisResult.Documents);
                _CEDAlfaiaAppContextDatabase.PieceVenteLignes.RemoveRange(devisResult.Lignes);
                _CEDAlfaiaAppContextDatabase.PieceVenteServices.RemoveRange(devisResult.Services);
                if (devisResult.RendezVous != null)
                    _CEDAlfaiaAppContextDatabase.RendezVous.Remove(devisResult.RendezVous);

                _CEDAlfaiaAppContextDatabase.PiecesVente.Remove(devisResult);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }

        public void Init(List<Client> clients, List<Marque> marques, List<Modele> modeles, List<Voiture> voitures)
        {
            _CEDAlfaiaAppContextDatabase.Clients.AddRange(clients);
            _CEDAlfaiaAppContextDatabase.Marques.AddRange(marques);
            _CEDAlfaiaAppContextDatabase.Modeles.AddRange(modeles);
            _CEDAlfaiaAppContextDatabase.Voitures.AddRange(voitures);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }

        public PieceVente AddUpdateDevis(int pID, int pVoitureID, DateTime pDate, float pMainDOeuvre, int pDureeMainDOeuvre, int pKilometrage, float? pRemise, int? pIDRendezVous = null, bool pIsDevisEnvoye = false)
        {
            PieceVente devisResult = new PieceVente();
            if (pID > 0)
                devisResult = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(c => c.ID == pID).FirstOrDefault();

            devisResult.VoitureID = pVoitureID;

            Voiture voitureFound = _CEDAlfaiaAppContextDatabase.Voitures.Where(v => v.ID == pVoitureID).FirstOrDefault();
            if (voitureFound != null)
                devisResult.ClientID = voitureFound.ClientID;
            devisResult.DateDevis = pDate;
            devisResult.MainDOeuvreMontantHoraire = pMainDOeuvre;
            devisResult.MainDOeuvreDuree = pDureeMainDOeuvre;
            devisResult.Kilometrage = pKilometrage;
            devisResult.Remise = pRemise;
            devisResult.IsDevisEnvoye = pIsDevisEnvoye;

            if (pIDRendezVous != null && pIDRendezVous > 0)
                devisResult.RendezVousID = pIDRendezVous.Value;

            if (pID == 0)
            {
                devisResult.NumDevis = GetNextNumDevis();
                devisResult = _CEDAlfaiaAppContextDatabase.PiecesVente.Add(devisResult);
            }

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return devisResult;
        }
        public int GetNextNumDevis()
        {
            int numDevisInitial = int.Parse(DateTime.Now.Year.ToString().Substring(2, 2) + "000000");
            int? lastNumDevis = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(d => d.NumDevis != null && d.NumDevis > numDevisInitial).Max(d => d.NumDevis);
            if (lastNumDevis != null)
                return lastNumDevis.Value + 1;
            return numDevisInitial + 1;
        }
        #endregion

        public void UpdatePiecesVente()
        {
            foreach (var pv in _CEDAlfaiaAppContextDatabase.PiecesVente.ToList())
            {
                float totalHT = pv.CalculerTotalHT();
                float totalTTC = pv.CalculerTotalTCC();
                float beneficeTTC = pv.CalculerBeneficeTTC();
                float resteAPayer = pv.CalculerResteAPayer();
                float montantTVA = pv.CalculerMontantTVA();
                pv.TotalHT = totalHT;
                pv.TotalTTC = totalTTC;
                pv.BeneficeTTC = beneficeTTC;
                pv.ResteAPayer = resteAPayer;
                pv.MontantTVA = montantTVA;
            }
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public void UpdateTotauxPieceVente(int pID)
        {
            PieceVente pv = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(p => p.ID == pID).FirstOrDefault();
            if (pv != null)
            {
                pv.TotalHT = pv.CalculerTotalHT();
                pv.TotalTTC = pv.CalculerTotalTCC();
                pv.BeneficeTTC = pv.CalculerBeneficeTTC();
                pv.ResteAPayer = pv.CalculerResteAPayer();
                pv.MontantTVA = pv.CalculerMontantTVA();
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }

        #region Lignes Devis
        public PieceVenteLigne AddUpdateLigneDevis(int pID, int pPieceVenteID, int? pFournisseurID, string pLibelle, float pPrixGarageHT, float pPrixGarageTTC, float pPrixClientHT, float pPrixClientTTC, int pQuantite, float pRemise)
        {
            PieceVenteLigne ligneDevisResult = new PieceVenteLigne();
            if (pID > 0)
                ligneDevisResult = _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Where(c => c.ID == pID).FirstOrDefault();

            ligneDevisResult.Libelle = pLibelle;
            ligneDevisResult.PieceVenteID = pPieceVenteID;
            ligneDevisResult.FournisseurID = pFournisseurID;
            ligneDevisResult.PrixClientHT = pPrixClientHT;
            ligneDevisResult.PrixClientTTC = pPrixClientTTC;
            ligneDevisResult.PrixGarageHT = pPrixGarageHT;
            ligneDevisResult.PrixGarageTTC = pPrixGarageTTC;
            ligneDevisResult.Quantite = pQuantite;
            ligneDevisResult.Remise = pRemise;

            if (pID == 0)
                ligneDevisResult = _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Add(ligneDevisResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return ligneDevisResult;
        }
        public void DeleteLigneDevis(int pID)
        {
            PieceVenteLigne pieceVenteLigneResult = _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Where(c => c.ID == pID).FirstOrDefault();
            if (pieceVenteLigneResult != null)
            {
                _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Remove(pieceVenteLigneResult);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
        #endregion

        #region Services Pièces de vente
        public PieceVenteService AddUpdateServicePieceVente(int pID, int pPieceVenteID, string pLibelle, float pPrixClientHT, float pPrixClientTTC, int pQuantite)
        {
            PieceVenteService service = new PieceVenteService();
            if (pID > 0)
                service = _CEDAlfaiaAppContextDatabase.PieceVenteServices.Where(c => c.ID == pID).FirstOrDefault();

            service.Libelle = pLibelle;
            service.PieceVenteID = pPieceVenteID;
            service.PrixClientHT = pPrixClientHT;
            service.PrixClientTTC = pPrixClientTTC;
            service.Quantite = pQuantite;

            if (pID == 0)
                service = _CEDAlfaiaAppContextDatabase.PieceVenteServices.Add(service);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return service;
        }
        public void DeleteServicePieceVente(int pID)
        {
            PieceVenteService pieceVenteServiceResult = _CEDAlfaiaAppContextDatabase.PieceVenteServices.Where(c => c.ID == pID).FirstOrDefault();
            if (pieceVenteServiceResult != null)
            {
                _CEDAlfaiaAppContextDatabase.PieceVenteServices.Remove(pieceVenteServiceResult);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
        #endregion

        #region Factures

        public IQueryable<PieceVente> Factures()
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(p => p.NumFacture != null && !p.IsFactureAnnule);
        }
        public List<PieceVente> GetFactures()
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumFacture != null && !pv.IsFactureAnnule).ToList();
        }
        public List<PieceVente> GetFacturesAEnvoyer()
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumFacture != null && !pv.IsFactureAnnule && pv.IsValide && !pv.IsEnvoyeComptable).ToList();
        }
        public IQueryable<PieceVente> GetFacturesAnnulees()
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumFacture != null && pv.IsFactureAnnule);
        }
        public List<PieceVente> GetFacturesBetweenDates(DateTime pDateDebut, DateTime pDateFin)
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumFacture != null && pv.DateFacture >= pDateDebut && pv.DateFacture <= pDateFin).ToList();
        }
        public List<PieceVente> GetFacturesValideesBetweenDates(DateTime pDateDebut, DateTime pDateFin)
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumFacture != null && pv.DateFacture >= pDateDebut && pv.IsValide).ToList();
        }
        public PieceVente GetPieceVente(int id)
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.ID == id).FirstOrDefault();
        }
        public PieceVente GetFacture(int id)
        {
            return _CEDAlfaiaAppContextDatabase.PiecesVente.Where(pv => pv.NumFacture != null && pv.ID == id).FirstOrDefault();
        }
        public PieceVente AddUpdateFacture(int pID, int pVoitureID, DateTime pDate, float pMainDOeuvre, int pDureeMainDOeuvre, int pKilometrage, float? pRemise, int? pIDRendezVous = null)
        {
            PieceVente factureResult = new PieceVente();
            if (pID > 0)
                factureResult = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(c => c.ID == pID).FirstOrDefault();

            factureResult.VoitureID = pVoitureID;

            Voiture voitureFound = _CEDAlfaiaAppContextDatabase.Voitures.Where(v => v.ID == pVoitureID).FirstOrDefault();
            if (voitureFound != null)
                factureResult.ClientID = voitureFound.ClientID;
            factureResult.DateFacture = pDate;
            factureResult.MainDOeuvreMontantHoraire = pMainDOeuvre;
            factureResult.MainDOeuvreDuree = pDureeMainDOeuvre;
            factureResult.Kilometrage = pKilometrage;
            factureResult.Remise = pRemise;

            if (pIDRendezVous != null)
                factureResult.RendezVousID = pIDRendezVous.Value;

            if (pID == 0)
            {
                factureResult.NumFacture = GetNextNumFacture();
                factureResult.DateFacture = DateTime.Now;
                factureResult = _CEDAlfaiaAppContextDatabase.PiecesVente.Add(factureResult);
            }

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return factureResult;
        }
        public void AnnulerFacture(int pIDFacture, string pCommentaire)
        {
            PieceVente factureResult = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(c => c.ID == pIDFacture).FirstOrDefault();
            if (factureResult == null)
            {
                throw new Exception("Facture introuvable !");
            }
            factureResult.CommentaireAnnulation = pCommentaire;
            factureResult.IsFactureAnnule = true;
            factureResult.DateHeureAnnulation = DateTime.Now;

            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public void RepasserEnFacture(int pID)
        {
            PieceVente factureResult = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(c => c.ID == pID).FirstOrDefault();
            if (factureResult != null)
            {
                factureResult.IsFactureAnnule = false;
                factureResult.CommentaireAnnulation = null;

                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
        public PieceVente GenererFactureByIDDevis(int pIDDevis)
        {
            PieceVente newFacture = GetDevis(pIDDevis);

            if (newFacture != null)
            {
                newFacture.NumFacture = GetNextNumFacture();
                newFacture.DateFacture = DateTime.Now;
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }

            return newFacture;
        }
        public PieceVente ValiderFacture(int pIdFacture)
        {
            PieceVente facture = GetFacture(pIdFacture);

            if (facture != null)
            {
                facture.IsValide = true;
                facture.DateHeureValidation = DateTime.Now;
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }

            return facture;
        }
        public PieceVente PayerFacture(int pIdFacture, TypePaiement modePaiement)
        {
            PieceVente facture = GetFacture(pIdFacture);
            if (facture != null)
            {
                _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Add(new PieceVentePaiement()
                {
                    Date = DateTime.Now,
                    Montant = facture.ResteAPayer,
                    TypePaiement = modePaiement,
                    PieceVenteID = facture.ID
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
                UpdateTotauxPieceVente(facture.ID);
            }
            return facture;
        }
        public PieceVente PayerPieceVente(int pIDPieceVente, TypePaiement modePaiement)
        {
            PieceVente pieceVente = GetPieceVente(pIDPieceVente);
            if (pieceVente != null)
            {
                _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Add(new PieceVentePaiement()
                {
                    Date = DateTime.Now,
                    Montant = pieceVente.ResteAPayer,
                    TypePaiement = modePaiement,
                    PieceVenteID = pieceVente.ID
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
                UpdateTotauxPieceVente(pieceVente.ID);
            }
            return pieceVente;
        }
        public int GetNextNumFacture()
        {
            int numFactureInitial = int.Parse(DateTime.Now.Year.ToString().Substring(2, 2) + "000000");
            int? lastNumFacture = _CEDAlfaiaAppContextDatabase.PiecesVente.Where(d => d.NumFacture != null && d.NumFacture > numFactureInitial).Max(d => d.NumFacture);
            if (lastNumFacture != null)
                return lastNumFacture.Value + 1;
            return numFactureInitial + 1;
        }
        public void SetFacturesEnvoyes(List<int> pIds)
        {
            foreach (int id in pIds)
            {
                PieceVente facture = GetFacture(id);
                if (facture != null)
                    facture.IsEnvoyeComptable = true;
            }
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        #endregion

        #region Lignes Facture
        public PieceVenteLigne AddUpdateLigneFacture(int pID, int pPieceVenteID, int? pFournisseurID, string pLibelle, float pPrixGarageHT, float pPrixGarageTTC, float pPrixClientHT, float pPrixClientTTC, int pQuantite, float pRemise)
        {
            PieceVenteLigne ligneFactureResult = new PieceVenteLigne();
            if (pID > 0)
                ligneFactureResult = _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Where(c => c.ID == pID).FirstOrDefault();

            ligneFactureResult.Libelle = pLibelle;
            ligneFactureResult.PieceVenteID = pPieceVenteID;
            ligneFactureResult.FournisseurID = pFournisseurID;
            ligneFactureResult.PrixClientHT = pPrixClientHT;
            ligneFactureResult.PrixClientTTC = pPrixClientTTC;
            ligneFactureResult.PrixGarageHT = pPrixGarageHT;
            ligneFactureResult.PrixGarageTTC = pPrixGarageTTC;
            ligneFactureResult.Quantite = pQuantite;
            ligneFactureResult.Remise = pRemise;

            if (pID == 0)
                ligneFactureResult = _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Add(ligneFactureResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return ligneFactureResult;
        }
        public void DeleteLigneFacture(int pID)
        {
            PieceVenteLigne pieceVenteLigneResult = _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Where(c => c.ID == pID).FirstOrDefault();
            if (pieceVenteLigneResult != null)
            {
                _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Remove(pieceVenteLigneResult);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
        #endregion

        public IEnumerable<string> GetAllDesignations()
        {
            return _CEDAlfaiaAppContextDatabase.PieceVenteLignes.Select(l => l.Libelle).OrderBy(l => l);
        }

        #region Documents
        public DocumentPieceVente AddUpdateDocument(int pID, int? pIDPieceVente, string pLibelle, TypeDocument pTypeDoc, byte[] pDocument, string pDocumentFormatFichierBase64)
        {
            DocumentPieceVente documentResult = new DocumentPieceVente();
            if (pID > 0)
            {
                documentResult = _CEDAlfaiaAppContextDatabase.Documents.Where(c => c.ID == pID).FirstOrDefault();
                documentResult.DateModification = DateTime.Now;
            }
            else
                documentResult.DateCreation = DateTime.Now;

            documentResult.PieceVenteID = pIDPieceVente;
            documentResult.TypeDocument = pTypeDoc;
            documentResult.Libelle = pLibelle;
            documentResult.Doc = pDocument;
            documentResult.DocFormatFichierBase64 = pDocumentFormatFichierBase64;

            if (pID == 0)
                documentResult = _CEDAlfaiaAppContextDatabase.Documents.Add(documentResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return documentResult;
        }
        public DocumentPieceVente AddUpdateDocument(DocumentPieceVente pDocument)
        {
            DocumentPieceVente documentResult = new DocumentPieceVente();
            if (pDocument.ID > 0)
            {
                documentResult = _CEDAlfaiaAppContextDatabase.Documents.Where(c => c.ID == pDocument.ID).FirstOrDefault();
                documentResult.DateModification = DateTime.Now;
            }
            else
                documentResult.DateCreation = DateTime.Now;

            documentResult.PieceVenteID = pDocument.PieceVenteID;
            documentResult.TypeDocument = pDocument.TypeDocument;
            documentResult.Libelle = pDocument.Libelle;
            documentResult.Doc = pDocument.Doc;
            documentResult.DocFormatFichierBase64 = pDocument.DocFormatFichierBase64;

            if (pDocument.ID == 0)
                documentResult = _CEDAlfaiaAppContextDatabase.Documents.Add(documentResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return documentResult;
        }
        public void DeleteDocument(int pID)
        {
            DocumentPieceVente documentResult = _CEDAlfaiaAppContextDatabase.Documents.Where(c => c.ID == pID).FirstOrDefault();
            if (documentResult != null)
            {
                _CEDAlfaiaAppContextDatabase.Documents.Remove(documentResult);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
        public DocumentPieceVente AddUpdateDocument(int pIDPieceVente, TypeDocument pTypeDocument, string pLibelle, byte[] pDocument, string pDocumentFormatFichierBase64)
        {
            DocumentPieceVente documentResult = _CEDAlfaiaAppContextDatabase.Documents.Where(d => d.PieceVenteID == pIDPieceVente && d.TypeDocument == pTypeDocument).FirstOrDefault();
            if (documentResult == null)
            {
                documentResult = new DocumentPieceVente();
                documentResult.DateCreation = DateTime.Now;
            }
            else
                documentResult.DateModification = DateTime.Now;

            documentResult.Doc = pDocument;
            documentResult.DocFormatFichierBase64 = pDocumentFormatFichierBase64;
            documentResult.TypeDocument = pTypeDocument;
            documentResult.Libelle = pLibelle;
            documentResult.PieceVenteID = pIDPieceVente;

            if (documentResult.ID == 0)
                documentResult = _CEDAlfaiaAppContextDatabase.Documents.Add(documentResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return documentResult;
        }
        #endregion


        #region Paiements de pièce de vente
        public PieceVentePaiement AddUpdatePieceVentePaiement(int pID, int pPieceVenteID, DateTime pDate, float pMontant, TypePaiement pTypePaiement)
        {
            PieceVentePaiement paiementResult = paiementResult = _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Where(c => c.ID == pID).FirstOrDefault();

            if (paiementResult == null)
                paiementResult = new PieceVentePaiement();

            paiementResult.Date = pDate;
            paiementResult.Montant = pMontant;
            paiementResult.PieceVenteID = pPieceVenteID;
            paiementResult.TypePaiement = pTypePaiement;

            paiementResult = _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Add(paiementResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();

            return paiementResult;
        }
        public void DeletePaiementPieceVente(int pID)
        {
            PieceVentePaiement paiementResult = _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Where(c => c.ID == pID).FirstOrDefault();
            if (paiementResult != null)
            {
                _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Remove(paiementResult);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
        public void DeletePaiementsPieceVente(int pIDPiece)
        {
            foreach (PieceVentePaiement p in _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Where(c => c.PieceVenteID == pIDPiece))
            {
                if (p != null)
                    _CEDAlfaiaAppContextDatabase.PieceVentePaiements.Remove(p);
            }
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        #endregion

        #region Rendez-vous
        public RendezVous AddUpdateRendezVous(int pID, string pSujet, int pDuree, DateTime pDateHeureDebut, DateTime pDateHeureFin, string pCommentaire)
        {
            RendezVous rdvResult = new RendezVous();
            if (pID > 0)
                rdvResult = _CEDAlfaiaAppContextDatabase.RendezVous.Where(c => c.ID == pID).FirstOrDefault();

            rdvResult.Sujet = pSujet;
            rdvResult.Duree = pDuree;
            rdvResult.DateHeureDebut = pDateHeureDebut;
            rdvResult.DateHeureFin = pDateHeureFin;
            rdvResult.Commentaire = pCommentaire;

            if (pID == 0)
                rdvResult = _CEDAlfaiaAppContextDatabase.RendezVous.Add(rdvResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return rdvResult;
        }
        public List<RendezVous> GetRendezVous()
        {
            return _CEDAlfaiaAppContextDatabase.RendezVous.ToList();
        }
        public List<RendezVous> GetRendezVous(int mois, int annee)
        {
            DateTime dateDebut = new DateTime(annee, mois, 1);
            DateTime dateFin = new DateTime(annee, mois, DateTime.DaysInMonth(annee, mois));
            return _CEDAlfaiaAppContextDatabase.RendezVous.Where(rdv => rdv.DateHeureDebut >= dateDebut && rdv.DateHeureFin <= dateFin).ToList();
        }
        #endregion

        #region Comptabilite
        public List<BalanceAgee> BalanceAgee()
        {
            List<BalanceAgee> ba = new List<DAL.Garage.BalanceAgee>();

            foreach (var client in _CEDAlfaiaAppContextDatabase.Clients.Where(c => !c.IsProspect).ToList())
            {
                if (client.PiecesVente.Any(pv => pv.DateHeureAnnulation != null && pv.NumFacture != null && pv.ResteAPayer > 0))
                {
                    BalanceAgee balance = new DAL.Garage.BalanceAgee();
                    balance.NomClient = ((!string.IsNullOrEmpty(client.Prenom) ? client.Prenom + " " : "") + (!string.IsNullOrEmpty(client.Nom) ? client.Nom + " " : "")).Trim();
                    balance.SeptJours = client.PiecesVente.Where(pv => pv.DateFacture <= DateTime.Now && pv.DateFacture >= DateTime.Now.AddDays(-7)).Sum(pv => pv.ResteAPayer);
                    balance.QuinzeJours = client.PiecesVente.Where(pv => pv.DateFacture < DateTime.Now.AddDays(-7) && pv.DateFacture >= DateTime.Now.AddDays(-15)).Sum(pv => pv.ResteAPayer);
                    balance.UnMois = client.PiecesVente.Where(pv => pv.DateFacture < DateTime.Now.AddDays(-15) && pv.DateFacture >= DateTime.Now.AddDays(-30)).Sum(pv => pv.ResteAPayer);
                    balance.DeuxMois = client.PiecesVente.Where(pv => pv.DateFacture < DateTime.Now.AddDays(-30) && pv.DateFacture >= DateTime.Now.AddDays(-60)).Sum(pv => pv.ResteAPayer);
                    balance.TroisMois = client.PiecesVente.Where(pv => pv.DateFacture < DateTime.Now.AddDays(-60) && pv.DateFacture >= DateTime.Now.AddDays(-90)).Sum(pv => pv.ResteAPayer);
                    balance.Plus = client.PiecesVente.Where(pv => pv.DateFacture < DateTime.Now.AddDays(-90)).Sum(pv => pv.ResteAPayer);
                    balance.Total = balance.SeptJours + balance.QuinzeJours + balance.UnMois + balance.DeuxMois + balance.TroisMois + balance.Plus;

                    ba.Add(balance);
                }
            }

            return ba;
        }
        #endregion
    }
}
