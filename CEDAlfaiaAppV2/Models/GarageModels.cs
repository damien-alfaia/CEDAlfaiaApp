using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CEDAlfaiaAppV2.Models
{
    public class AdresseModel
    {
        [Display(Name = "Ligne 1")]
        public string Ligne1 { get; set; }
        [Display(Name = "Ligne 2")]
        public string Ligne2 { get; set; }
        [Display(Name = "Ligne 3")]
        public string Ligne3 { get; set; }
        [Display(Name = "Code postal")]
        public string CodePostal { get; set; }
        [Display(Name = "Ville")]
        public string Ville { get; set; }
        [Display(Name = "Pays")]
        public string Pays { get; set; }

        public AdresseModel()
        {

        }

        public AdresseModel(Adresse pAdresse)
        {
            this.Ligne1 = pAdresse.Ligne1;
            this.Ligne2 = pAdresse.Ligne2;
            this.Ligne3 = pAdresse.Ligne3;
            this.CodePostal = pAdresse.CodePostal;
            this.Ville = pAdresse.Ville;
            this.Pays = pAdresse.Pays;
        }
    }
    public class ClientModel
    {
        public int? ID { get; set; }
        [Display(Name = "Nom")]
        public string Nom { get; set; }
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Téléphone")]
        public string Telephone { get; set; }
        [Display(Name = "Compléments")]
        public string Complements { get; set; }
        [Display(Name = "Remise")]
        public float Remise { get; set; }

        [Display(Name = "Prospect ?")]
        public bool IsProspect { get; set; }

        [Display(Name = "Statut")]
        public string Statut
        {
            get
            {
                if (this.IsProspect)
                    return "Prospect";
                else
                    return "Client";
            }
        }

        [Display(Name = "Voitures")]
        public List<VoitureModel> Voitures { get; set; }

        [Display(Name = "Adresse")]
        public AdresseModel Adresse { get; set; }

        public ClientModel()
        {
            this.Voitures = new List<VoitureModel>();
            this.Adresse = new AdresseModel();
            this.Remise = 0;
            this.IsProspect = true;
        }

        public ClientModel(bool? isProspect)
        {
            this.Voitures = new List<VoitureModel>();
            this.Adresse = new AdresseModel();
            this.Remise = 0;
            if (isProspect == null)
                this.IsProspect = true;
            else
                this.IsProspect = isProspect.Value;
        }

        public ClientModel(Client pClient)
        {
            this.ID = pClient.ID;
            this.Nom = pClient.Nom;
            this.Prenom = pClient.Prenom;
            this.Telephone = pClient.Telephone;
            this.Email = pClient.Email;
            this.Complements = pClient.InformationsComplementaires;
            this.IsProspect = pClient.IsProspect;

            this.Adresse = new AdresseModel(pClient.Adresse);
            this.Voitures = pClient.Voitures.Select(v => new VoitureModel(v)).Where(v => !v.EstSupprime).ToList();

            this.Remise = 0;
            if (pClient.Remise != null)
                this.Remise = pClient.Remise.Value;
        }
    }

    public class VoitureModel
    {
        public int ID { get; set; }
        public int IDMarque { get; set; }
        public int IDModele { get; set; }
        public int IDClient { get; set; }
        [Display(Name = "Marque")]
        public string Marque { get; set; }
        [Display(Name = "Modèle")]
        public string Modele { get; set; }
        [Display(Name = "Immatriculation")]
        public string Immatriculation { get; set; }
        public bool EstSupprime { get; set; }

        public VoitureModel()
        {

        }

        public VoitureModel(int pIDClient)
        {
            this.IDClient = pIDClient;
        }

        public VoitureModel(Voiture pVoiture)
        {
            this.ID = pVoiture.ID;
            this.IDMarque = pVoiture.Modele.MarqueID.Value;
            this.IDModele = pVoiture.ModeleID.Value;
            this.IDClient = pVoiture.ClientID.Value;
            this.Marque = pVoiture.Modele.Marque.Libelle;
            this.Modele = pVoiture.Modele.Libelle;
            this.Immatriculation = pVoiture.Immatriculation;
            this.EstSupprime = pVoiture.DateSuppression != null;
        }
    }

    public class MarqueModel
    {
        public int IDMarque { get; set; }
        public string Valeur { get; set; }
        public string Libelle { get; set; }
        public List<ModeleModel> Modeles { get; set; }

        public MarqueModel()
        {
            this.Modeles = new List<ModeleModel>();
        }

        public MarqueModel(int id, string valeur, string libelle, List<ModeleModel> modeles)
        {
            this.IDMarque = id;
            this.Valeur = valeur;
            this.Libelle = libelle;
            this.Modeles = modeles;
        }

        public MarqueModel(Marque marque)
        {
            this.IDMarque = marque.ID;
            this.Valeur = marque.Code;
            this.Libelle = marque.Libelle;
            this.Modeles = marque.Modeles.Select(m => new ModeleModel(m)).OrderBy(m => m.Libelle).ToList();
        }
    }
    public class ModeleModel
    {
        public int IDModele { get; set; }
        public string Libelle { get; set; }

        public ModeleModel()
        {

        }

        public ModeleModel(int id, string libelle)
        {
            this.IDModele = id;
            this.Libelle = libelle;
        }

        public ModeleModel(Modele modele)
        {
            this.IDModele = modele.ID;
            this.Libelle = modele.Libelle;
        }
    }

    public class FournisseurModel
    {
        public int ID { get; set; }
        [Display(Name = "Nom")]
        public string Nom { get; set; }
        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }
        [Display(Name = "Adresse")]
        public AdresseModel Adresse { get; set; }

        public FournisseurModel()
        {
            this.Adresse = new AdresseModel();
        }

        public FournisseurModel(Fournisseur pFournisseur)
        {
            this.ID = pFournisseur.ID;
            this.Nom = pFournisseur.Nom;
            this.Commentaire = pFournisseur.Commentaire;
            this.Adresse = new AdresseModel(pFournisseur.Adresse);
        }
    }

    public class DevisModel
    {
        public bool IsAImprimer { get; set; }
        public int ID { get; set; }
        [Display(Name = "Numéro")]
        public int? Numero { get; set; }
        [Display(Name = "Client")]
        public ClientModel Client { get; set; }
        [Display(Name = "Client")]
        public string NomClient { get; set; }
        [Display(Name = "Voiture")]
        public VoitureModel Voiture { get; set; }
        [Display(Name = "Voiture")]
        public string NomVoiture { get; set; }
        [Display(Name = "Immatriculation")]
        public string Immatriculation { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Total TTC")]
        public double Total { get; set; }
        [Display(Name = "Total HT")]
        public double TotalHT { get; set; }
        [Display(Name = "Bénéfice TTC")]
        public double BeneficeTTC { get; set; }

        public DevisModel()
        {
        }
        public DevisModel(PieceVente pDevis)
        {
            this.ID = pDevis.ID;
            this.Numero = pDevis.NumDevis;
            this.Client = new ClientModel(pDevis.Client);
            this.Voiture = new VoitureModel(pDevis.Voiture);
            this.Date = DateTime.Now;
            if (pDevis.DateDevis != null)
                this.Date = pDevis.DateDevis.Value;
            this.Total = pDevis.TotalTTC;
            this.TotalHT = pDevis.TotalHT;
            this.BeneficeTTC = pDevis.BeneficeTTC;
            this.IsAImprimer = false;
            if (pDevis.Client != null)
                this.NomClient = (!string.IsNullOrEmpty(pDevis.Client.Prenom) ? pDevis.Client.Prenom.Trim() + " " : "") + (!string.IsNullOrEmpty(pDevis.Client.Nom) ? pDevis.Client.Nom.Trim() + " " : "");
            if (pDevis.Voiture != null)
            {
                this.NomVoiture = pDevis.Voiture.Modele.Marque.Libelle + " " + pDevis.Voiture.Modele.Libelle;
                this.Immatriculation = pDevis.Voiture.Immatriculation;
            }
        }
    }

    public class LigneDevisModel
    {
        public int ID { get; set; }
        [Display(Name = "Prix")]
        public double PrixTTC { get; set; }

        public LigneDevisModel()
        {

        }
        public LigneDevisModel(PieceVenteLigne pLigne)
        {
            this.ID = pLigne.ID;
            this.PrixTTC = pLigne.PrixClientTTC * pLigne.Quantite;
        }
    }

    public class SaisirDevisModel
    {
        public int IDDevis { get; set; }
        public int IDClient { get; set; }
        public string LibelleClient { get; set; }
        [Display(Name = "Client")]
        public int IDVoiture { get; set; }
        public string LibelleVoiture { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        public string DevisFornisseurBase64 { get; set; }
        public string BonLivraisonFornisseurBase64 { get; set; }
        [Display(Name = "Main d'oeuvre")]
        public float MainDOeuvre { get; set; }
        [Display(Name = "Durée main d'oeuvre")]
        public int DureeMainDOeuvre { get; set; }
        [Display(Name = "Total")]
        public float TotalMainDOeuvre
        {
            get
            {
                return MainDOeuvre * DureeMainDOeuvre;
            }
        }

        [Display(Name = "Remise")]
        public float Remise { get; set; }

        [Display(Name = "Total HT")]
        public float TotalHT { get; set; }
        [Display(Name = "Montant TVA")]
        public float MontantTVA { get; set; }
        [Display(Name = "Total TTC")]
        public float TotalTTC { get; set; }

        [Display(Name = "Bénéfice")]
        public float Benefice
        {
            get
            {
                return (this.Lignes.Sum(l => l.PrixClientTTC) - this.Lignes.Sum(l => l.PrixGarageTTC)) + this.TotalMainDOeuvre;
            }
        }
        public List<SaisirLignesDevisModel> Lignes { get; set; }
        public List<SaisirServicesModel> Services { get; set; }
        public List<SaisirPaiementPieceVenteModel> Paiements { get; set; }
        public RendezVousModel RendezVous { get; set; }
        public int Kilometrage { get; set; }

        public SaisirDevisModel()
        {
            this.Date = DateTime.Now.ToShortDateString();
            this.Lignes = new List<SaisirLignesDevisModel>();
            this.Lignes.Add(new SaisirLignesDevisModel(true));
            this.Services = new List<SaisirServicesModel>();
            this.Services.Add(new SaisirServicesModel(true));
            this.RendezVous = new RendezVousModel();
            this.Paiements = new List<SaisirPaiementPieceVenteModel>();
        }

        public SaisirDevisModel(PieceVente pDevis)
        {
            this.IDDevis = pDevis.ID;
            this.IDClient = pDevis.ClientID.Value;
            if (pDevis.Client != null)
                this.LibelleClient = pDevis.Client.Prenom + " " + pDevis.Client.Nom;
            this.IDVoiture = pDevis.VoitureID.Value;
            if (pDevis.Voiture != null)
                this.LibelleVoiture = pDevis.Voiture.Modele.Marque.Libelle + " " + pDevis.Voiture.Modele.Libelle + " - " + pDevis.Voiture.Immatriculation;
            this.Date = DateTime.Now.ToShortDateString();
            if (pDevis.DateDevis != null)
                this.Date = pDevis.DateDevis.Value.ToShortDateString();
            this.Lignes = new List<SaisirLignesDevisModel>();
            this.Lignes.Add(new SaisirLignesDevisModel(true));
            this.Lignes.AddRange(pDevis.Lignes.Select(l => new SaisirLignesDevisModel(l)).ToList());

            this.Services = new List<SaisirServicesModel>();
            this.Services.Add(new SaisirServicesModel(true));
            this.Services.AddRange(pDevis.Services.Select(s => new SaisirServicesModel(s)).ToList());

            DocumentPieceVente devisFournisseur = pDevis.Documents.Where(d => d.TypeDocument == TypeDocument.DevisFournisseur).FirstOrDefault();
            if (devisFournisseur != null)
                this.DevisFornisseurBase64 = Convert.ToBase64String(devisFournisseur.Doc);

            DocumentPieceVente bonLivraisonFournisseur = pDevis.Documents.Where(d => d.TypeDocument == TypeDocument.BonLivraisonFournisseur).FirstOrDefault();
            if (bonLivraisonFournisseur != null)
                this.BonLivraisonFornisseurBase64 = Convert.ToBase64String(bonLivraisonFournisseur.Doc);

            this.Paiements = new List<SaisirPaiementPieceVenteModel>();
            this.Paiements.AddRange(pDevis.Paiements.Select(p => new SaisirPaiementPieceVenteModel(p)).ToList());

            this.MainDOeuvre = pDevis.MainDOeuvreMontantHoraire;
            this.DureeMainDOeuvre = pDevis.MainDOeuvreDuree;
            this.RendezVous = new RendezVousModel(pDevis.RendezVous);
            this.Kilometrage = pDevis.Kilometrage;

            this.Remise = 0;
            if (pDevis.Remise != null)
                this.Remise = pDevis.Remise.Value;
            this.TotalHT = pDevis.TotalHT;
            this.MontantTVA = pDevis.MontantTVA;
            this.TotalTTC = pDevis.TotalTTC;
        }
    }

    public class SaisirLignesDevisModel
    {
        public int IDLigne { get; set; }
        public string Libelle { get; set; }
        public int Quantite { get; set; }
        public float Remise { get; set; }
        public float PrixGarageHT { get; set; }
        public float PrixGarageTTC { get; set; }
        public float PrixClientHT { get; set; }
        public float PrixClientTTC { get; set; }
        public bool IsSupprime { get; set; }
        public bool IsEmpty { get; set; }
        public int IndexLigne { get; set; }

        public SaisirLignesDevisModel()
        {
            this.Quantite = 1;
        }

        public SaisirLignesDevisModel(bool pIsEmpty)
        {
            this.IsEmpty = pIsEmpty;
            this.Quantite = 1;
        }

        public SaisirLignesDevisModel(PieceVenteLigne pLigne)
        {
            this.IDLigne = pLigne.ID;
            this.Libelle = pLigne.Libelle;
            this.Quantite = pLigne.Quantite;
            this.Remise = pLigne.Remise;
            this.PrixGarageHT = pLigne.PrixGarageHT;
            this.PrixGarageTTC = pLigne.PrixGarageTTC;
            this.PrixClientHT = pLigne.PrixClientHT;
            this.PrixClientTTC = pLigne.PrixClientTTC;
            this.IsSupprime = false;
            this.IsEmpty = false;
        }

        public SaisirLignesDevisModel(int pIDLigne, string pLibelle, int pQuantite, float pRemise, float pPrixGarageHT, float pPrixGarageTTC, float pPrixClientHT, float pPrixClientTTC, int pIndexLigne, bool pIsSupprime, bool pIsEmpty)
        {
            this.IDLigne = pIDLigne;
            this.Libelle = pLibelle;
            this.Quantite = pQuantite;
            this.Remise = pRemise;
            this.PrixGarageHT = pPrixGarageHT;
            this.PrixGarageTTC = pPrixGarageTTC;
            this.PrixClientHT = pPrixClientHT;
            this.PrixClientTTC = pPrixClientTTC;
            this.IsSupprime = pIsSupprime;
            this.IsEmpty = pIsEmpty;
            this.IndexLigne = pIndexLigne;
        }
    }

    public class SaisirServicesModel
    {
        public int IDService { get; set; }
        public string Libelle { get; set; }
        public int Quantite { get; set; }
        public float PrixClientHT { get; set; }
        public float PrixClientTTC { get; set; }
        public bool IsSupprime { get; set; }
        public bool IsEmpty { get; set; }
        public int IndexService { get; set; }
        public bool IsReadOnly { get; set; }

        public SaisirServicesModel()
        {
            this.Quantite = 1;
        }

        public SaisirServicesModel(bool pIsEmpty)
        {
            this.IsEmpty = pIsEmpty;
            this.Quantite = 1;
        }

        public SaisirServicesModel(PieceVenteService pService)
        {
            this.IDService = pService.ID;
            this.Libelle = pService.Libelle;
            this.Quantite = pService.Quantite;
            this.PrixClientHT = pService.PrixClientHT;
            this.PrixClientTTC = pService.PrixClientTTC;
            this.IsSupprime = false;
            this.IsEmpty = false;
            this.IsReadOnly = pService.PieceVente.IsValide;
        }

        public SaisirServicesModel(int pIDService, string pLibelle, int pQuantite, float pPrixClientHT, float pPrixClientTTC, int pIndexService, bool pIsSupprime, bool pIsEmpty)
        {
            this.IDService = pIDService;
            this.Libelle = pLibelle;
            this.Quantite = pQuantite;
            this.PrixClientHT = pPrixClientHT;
            this.PrixClientTTC = pPrixClientTTC;
            this.IsSupprime = pIsSupprime;
            this.IsEmpty = pIsEmpty;
            this.IndexService = pIndexService;
        }
    }

    public class FactureModel
    {
        public int ID { get; set; }
        [Display(Name = "Numéro")]
        public int? Numero { get; set; }
        [Display(Name = "Client")]
        public ClientModel Client { get; set; }
        [Display(Name = "Voiture")]
        public VoitureModel Voiture { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Lignes")]
        public List<LigneDevisModel> Lignes { get; set; }
        [Display(Name = "Total")]
        public double Total { get; set; }
        public bool IsAImprimer { get; set; }
        public bool IsAEnvoyer { get; set; }
        public string MotifAnnulation { get; set; }

        public FactureModel()
        {
            this.Lignes = new List<LigneDevisModel>();
        }
        public FactureModel(PieceVente pFacture)
        {
            this.ID = pFacture.ID;
            this.Numero = pFacture.NumFacture;
            this.Client = new ClientModel(pFacture.Client);
            this.Voiture = new VoitureModel(pFacture.Voiture);
            this.Date = DateTime.Now;
            if (pFacture.DateFacture != null)
                this.Date = pFacture.DateFacture.Value;
            this.Lignes = pFacture.Lignes.Select(l => new LigneDevisModel(l)).ToList();
            this.Total = pFacture.TotalTTC;
            this.IsAImprimer = false;
            this.MotifAnnulation = pFacture.CommentaireAnnulation;
            this.IsAEnvoyer = false;
        }
    }

    public class LigneFactureModel
    {
        public int ID { get; set; }
        [Display(Name = "Prix")]
        public double PrixTTC { get; set; }

        public LigneFactureModel()
        {

        }
        public LigneFactureModel(PieceVenteLigne pLigne)
        {
            this.ID = pLigne.ID;
            this.PrixTTC = pLigne.PrixClientTTC * pLigne.Quantite;
        }
    }

    public class SaisirFactureModel
    {
        public int IDFacture { get; set; }
        public int IDClient { get; set; }
        public string LibelleClient { get; set; }
        [Display(Name = "Client")]
        public int IDVoiture { get; set; }
        public string LibelleVoiture { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        public string DevisFornisseurBase64 { get; set; }
        public string BonLivraisonFornisseurBase64 { get; set; }
        [Display(Name = "Montant de main d'oeuvre")]
        public float MainDOeuvre { get; set; }
        [Display(Name = "Durée main d'oeuvre")]
        public int DureeMainDOeuvre { get; set; }
        [Display(Name = "Total main d'oeuvre")]
        public float TotalMainDOeuvre
        {
            get
            {
                return MainDOeuvre * DureeMainDOeuvre;
            }
        }
        [Display(Name = "Remise")]
        public float Remise { get; set; }

        [Display(Name = "Total HT")]
        public float TotalHT { get; set; }
        [Display(Name = "Montant TVA")]
        public float MontantTVA { get; set; }
        [Display(Name = "Total TTC")]
        public float TotalTTC { get; set; }

        [Display(Name = "Bénéfice")]
        public float Benefice
        {
            get
            {
                return (this.Lignes.Sum(l => l.PrixClientTTC) - this.Lignes.Sum(l => l.PrixGarageTTC)) + this.TotalMainDOeuvre;
            }
        }
        public int Kilometrage { get; set; }
        public List<SaisirLignesFactureModel> Lignes { get; set; }
        public List<SaisirServicesModel> Services { get; set; }
        public List<SaisirPaiementPieceVenteModel> Paiements { get; set; }
        public RendezVousModel RendezVous { get; set; }

        public bool IsValide { get; set; }
        public bool IsAnnule { get; set; }

        public SaisirFactureModel()
        {
            this.Date = DateTime.Now.ToShortDateString();
            this.Lignes = new List<SaisirLignesFactureModel>();
            this.Lignes.Add(new SaisirLignesFactureModel(true));
            this.Lignes.Add(new SaisirLignesFactureModel());
            this.Services = new List<SaisirServicesModel>();
            this.Services.Add(new SaisirServicesModel(true));
            this.Services.Add(new SaisirServicesModel());
            this.RendezVous = new RendezVousModel();
            this.Paiements = new List<SaisirPaiementPieceVenteModel>();
            this.IsValide = false;
            this.IsAnnule = false;
        }

        public SaisirFactureModel(PieceVente pFacture)
        {
            this.IDFacture = pFacture.ID;
            this.IDClient = pFacture.ClientID.Value;
            if (pFacture.Client != null)
                this.LibelleClient = pFacture.Client.Prenom + " " + pFacture.Client.Nom;
            this.IDVoiture = pFacture.VoitureID.Value;
            if (pFacture.Voiture != null)
                this.LibelleVoiture = pFacture.Voiture.Modele.Marque.Libelle + " " + pFacture.Voiture.Modele.Libelle + " - " + pFacture.Voiture.Immatriculation;
            this.Date = DateTime.Now.ToShortDateString();
            if (pFacture.DateDevis != null)
                this.Date = pFacture.DateDevis.Value.ToShortDateString();
            this.Lignes = new List<SaisirLignesFactureModel>();
            this.Lignes.Add(new SaisirLignesFactureModel(true));
            this.Lignes.AddRange(pFacture.Lignes.Select(l => new SaisirLignesFactureModel(l)).ToList());

            this.Services = new List<SaisirServicesModel>();
            this.Services.Add(new SaisirServicesModel(true));
            this.Services.AddRange(pFacture.Services.Select(s => new SaisirServicesModel(s)).ToList());

            DocumentPieceVente devisFournisseur = pFacture.Documents.Where(d => d.TypeDocument == TypeDocument.DevisFournisseur).FirstOrDefault();
            if (devisFournisseur != null)
                this.DevisFornisseurBase64 = Convert.ToBase64String(devisFournisseur.Doc);

            DocumentPieceVente bonLivraisonFournisseur = pFacture.Documents.Where(d => d.TypeDocument == TypeDocument.BonLivraisonFournisseur).FirstOrDefault();
            if (bonLivraisonFournisseur != null)
                this.BonLivraisonFornisseurBase64 = Convert.ToBase64String(bonLivraisonFournisseur.Doc);

            this.Paiements = new List<SaisirPaiementPieceVenteModel>();
            this.Paiements.AddRange(pFacture.Paiements.Select(p => new SaisirPaiementPieceVenteModel(p)).ToList());

            this.MainDOeuvre = pFacture.MainDOeuvreMontantHoraire;
            this.DureeMainDOeuvre = pFacture.MainDOeuvreDuree;
            this.RendezVous = new RendezVousModel(pFacture.RendezVous);
            this.Kilometrage = pFacture.Kilometrage;

            this.Remise = 0;
            if (pFacture.Remise != null)
                this.Remise = pFacture.Remise.Value;
            this.TotalHT = pFacture.TotalHT;
            this.MontantTVA = pFacture.MontantTVA;
            this.TotalTTC = pFacture.TotalTTC;

            this.IsValide = pFacture.IsValide;
            this.IsAnnule = pFacture.IsFactureAnnule;
        }
    }

    public class SaisirLignesFactureModel
    {
        public int IDLigne { get; set; }
        public string Libelle { get; set; }
        public int Quantite { get; set; }
        public float Remise { get; set; }
        public float PrixGarageHT { get; set; }
        public float PrixGarageTTC { get; set; }
        public float PrixClientHT { get; set; }
        public float PrixClientTTC { get; set; }
        public bool IsSupprime { get; set; }
        public bool IsEmpty { get; set; }
        public int IndexLigne { get; set; }
        public bool IsReadOnly { get; set; }

        public SaisirLignesFactureModel()
        {
            this.Quantite = 1;
        }

        public SaisirLignesFactureModel(bool pIsEmpty)
        {
            this.IsEmpty = pIsEmpty;
            this.Quantite = 1;
        }

        public SaisirLignesFactureModel(PieceVenteLigne pLigne)
        {
            this.IDLigne = pLigne.ID;
            this.Libelle = pLigne.Libelle;
            this.Quantite = pLigne.Quantite;
            this.Remise = pLigne.Remise;
            this.PrixGarageHT = pLigne.PrixGarageHT;
            this.PrixGarageTTC = pLigne.PrixGarageTTC;
            this.PrixClientHT = pLigne.PrixClientHT;
            this.PrixClientTTC = pLigne.PrixClientTTC;
            this.IsSupprime = false;
            this.IsEmpty = false;
            this.IsReadOnly = pLigne.PieceVente.IsValide;
        }

        public SaisirLignesFactureModel(int pIDLigne, string pLibelle, int pQuantite, float pRemise, float pPrixGarageHT, float pPrixGarageTTC, float pPrixClientHT, float pPrixClientTTC, int pIndexLigne, bool pIsSupprime, bool pIsEmpty)
        {
            this.IDLigne = pIDLigne;
            this.Libelle = pLibelle;
            this.Quantite = pQuantite;
            this.Remise = pRemise;
            this.PrixGarageHT = pPrixGarageHT;
            this.PrixGarageTTC = pPrixGarageTTC;
            this.PrixClientHT = pPrixClientHT;
            this.PrixClientTTC = pPrixClientTTC;
            this.IsSupprime = pIsSupprime;
            this.IsEmpty = pIsEmpty;
            this.IndexLigne = pIndexLigne;
        }
    }


    public class SaisirPieceVenteModel
    {
        public int IDPieceVente { get; set; }
        public int IDClient { get; set; }
        public string TypePiece { get; set; }
        //public int? IDClient { get; set; }
        public string LibelleClient { get; set; }
        [Display(Name = "Client")]
        public int? IDVoiture { get; set; }
        public string LibelleVoiture { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        public string DevisFornisseurBase64 { get; set; }
        public string BonLivraisonFornisseurBase64 { get; set; }
        [Display(Name = "Montant de main d'oeuvre")]
        public float MainDOeuvre { get; set; }
        [Display(Name = "Durée main d'oeuvre")]
        public int DureeMainDOeuvre { get; set; }
        [Display(Name = "Total main d'oeuvre")]
        public int Numero { get; set; }
        public float TotalMainDOeuvre
        {
            get
            {
                return MainDOeuvre * DureeMainDOeuvre;
            }
        }
        [Display(Name = "Remise")]
        public float Remise { get; set; }

        [Display(Name = "Total HT")]
        public float TotalHT { get; set; }
        [Display(Name = "Montant TVA")]
        public float MontantTVA { get; set; }
        [Display(Name = "Total TTC")]
        public float TotalTTC { get; set; }

        [Display(Name = "Bénéfice")]
        public float Benefice
        {
            get
            {
                float benefice = 0;
                foreach (var ligne in this.Lignes)
                {
                    if (ligne.PrixClientTTC != null && ligne.PrixGarageTTC != null)
                    {
                        benefice += float.Parse(ligne.PrixClientTTC.Replace('.', ',')) - float.Parse(ligne.PrixGarageTTC.Replace('.', ','));
                    }
                }
                benefice += this.TotalMainDOeuvre;
                return benefice;
            }
        }
        public int Kilometrage { get; set; }
        public List<SaisirLignesPieceVenteModel> Lignes { get; set; }
        public List<SaisirServicesModel> Services { get; set; }
        public List<SaisirPaiementPieceVenteModel> Paiements { get; set; }
        public RendezVousModel RendezVous { get; set; }

        public bool IsValide { get; set; }
        public bool IsAnnule { get; set; }

        [Display(Name = "Devis envoyé au client")]
        public bool IsDevisEnvoye { get; set; }

        public SaisirPieceVenteModel()
        {
            this.Date = DateTime.Now;
            this.Lignes = new List<SaisirLignesPieceVenteModel>();
            this.Services = new List<SaisirServicesModel>();
            this.Services.Add(new SaisirServicesModel(true));
            this.Services.Add(new SaisirServicesModel());
            this.RendezVous = new RendezVousModel();
            this.Paiements = new List<SaisirPaiementPieceVenteModel>();
            this.IsValide = false;
            this.IsAnnule = false;
            this.IsDevisEnvoye = false;
        }

        public SaisirPieceVenteModel(string typePieceVente)
        {
            this.Date = DateTime.Now;
            this.Lignes = new List<SaisirLignesPieceVenteModel>();
            this.Services = new List<SaisirServicesModel>();
            this.Services.Add(new SaisirServicesModel(true));
            this.Services.Add(new SaisirServicesModel());
            this.RendezVous = new RendezVousModel();
            this.Paiements = new List<SaisirPaiementPieceVenteModel>();
            this.IsValide = false;
            this.IsAnnule = false;
            this.IsDevisEnvoye = false;
            this.TypePiece = typePieceVente;
        }

        public SaisirPieceVenteModel(PieceVente pPieceVente)
        {
            this.IDPieceVente = pPieceVente.ID;
            this.TypePiece = "Devis";
            if (pPieceVente.DateFacture != null)
                this.TypePiece = "Facture";
            //this.IDClient = pPieceVente.ClientID.Value;
            if (pPieceVente.Client != null)
                this.LibelleClient = pPieceVente.Client.Prenom + " " + pPieceVente.Client.Nom;
            this.IDVoiture = pPieceVente.VoitureID;
            if (pPieceVente.Voiture != null)
                this.LibelleVoiture = pPieceVente.Voiture.Modele.Marque.Libelle + " " + pPieceVente.Voiture.Modele.Libelle + " - " + pPieceVente.Voiture.Immatriculation;
            this.Date = DateTime.Now;
            this.IDClient = pPieceVente.Voiture.ClientID.Value;
            if (TypePiece == "Devis")
            {
                this.Date = pPieceVente.DateDevis.Value;
                this.Numero = pPieceVente.NumDevis.Value;
            }
            else
            {
                this.Date = pPieceVente.DateFacture.Value;
                this.Numero = pPieceVente.NumFacture.Value;
            }
            this.Lignes = new List<SaisirLignesPieceVenteModel>();
            this.Lignes.AddRange(pPieceVente.Lignes.Select(l => new SaisirLignesPieceVenteModel(l)).ToList());

            this.Services = new List<SaisirServicesModel>();
            this.Services.Add(new SaisirServicesModel(true));
            this.Services.AddRange(pPieceVente.Services.Select(s => new SaisirServicesModel(s)).ToList());

            DocumentPieceVente devisFournisseur = pPieceVente.Documents.Where(d => d.TypeDocument == TypeDocument.DevisFournisseur).FirstOrDefault();
            if (devisFournisseur != null)
                this.DevisFornisseurBase64 = Convert.ToBase64String(devisFournisseur.Doc);

            DocumentPieceVente bonLivraisonFournisseur = pPieceVente.Documents.Where(d => d.TypeDocument == TypeDocument.BonLivraisonFournisseur).FirstOrDefault();
            if (bonLivraisonFournisseur != null)
                this.BonLivraisonFornisseurBase64 = Convert.ToBase64String(bonLivraisonFournisseur.Doc);

            this.Paiements = new List<SaisirPaiementPieceVenteModel>();
            this.Paiements.AddRange(pPieceVente.Paiements.Select(p => new SaisirPaiementPieceVenteModel(p)).ToList());

            this.MainDOeuvre = pPieceVente.MainDOeuvreMontantHoraire;
            this.DureeMainDOeuvre = pPieceVente.MainDOeuvreDuree;
            this.RendezVous = new RendezVousModel(pPieceVente.RendezVous);
            this.Kilometrage = pPieceVente.Kilometrage;

            this.Remise = 0;
            if (pPieceVente.Remise != null)
                this.Remise = pPieceVente.Remise.Value;
            this.TotalHT = pPieceVente.TotalHT;
            this.MontantTVA = pPieceVente.MontantTVA;
            this.TotalTTC = pPieceVente.TotalTTC;

            this.IsValide = pPieceVente.IsValide;
            this.IsAnnule = pPieceVente.IsFactureAnnule;
            this.IsDevisEnvoye = pPieceVente.IsDevisEnvoye;
        }
    }

    public class SaisirLignesPieceVenteModel
    {
        public int IDLigne { get; set; }
        public string Libelle { get; set; }
        public int Quantite { get; set; }
        public float Remise { get; set; }
        public string PrixGarageHT { get; set; }
        public string PrixGarageTTC { get; set; }
        public string PrixClientHT { get; set; }
        public string PrixClientTTC { get; set; }
        //public bool IsSupprime { get; set; }
        //public bool IsEmpty { get; set; }
        //public int IndexLigne { get; set; }
        //public bool IsReadOnly { get; set; }

        public SaisirLignesPieceVenteModel()
        {
            this.Quantite = 1;
        }

        public SaisirLignesPieceVenteModel(PieceVenteLigne pLigne)
        {
            this.IDLigne = pLigne.ID;
            this.Libelle = pLigne.Libelle;
            this.Quantite = pLigne.Quantite;
            this.Remise = pLigne.Remise;
            this.PrixGarageHT = pLigne.PrixGarageHT.ToString().Replace(',','.');
            this.PrixGarageTTC = pLigne.PrixGarageTTC.ToString().Replace(',', '.');
            this.PrixClientHT = pLigne.PrixClientHT.ToString().Replace(',', '.');
            this.PrixClientTTC = pLigne.PrixClientTTC.ToString().Replace(',', '.');
        }

        public SaisirLignesPieceVenteModel(int pIDLigne, string pLibelle, int pQuantite, float pRemise, float pPrixGarageHT, float pPrixGarageTTC, float pPrixClientHT, float pPrixClientTTC, int pIndexLigne, bool pIsSupprime, bool pIsEmpty)
        {
            this.IDLigne = pIDLigne;
            this.Libelle = pLibelle;
            this.Quantite = pQuantite;
            this.Remise = pRemise;
            this.PrixGarageHT = pPrixGarageHT.ToString().Replace(',', '.');
            this.PrixGarageTTC = pPrixGarageTTC.ToString().Replace(',', '.');
            this.PrixClientHT = pPrixClientHT.ToString().Replace(',', '.');
            this.PrixClientTTC = pPrixClientTTC.ToString().Replace(',', '.');
        }
    }

    public class ValeurLibelle
    {
        public int Valeur { get; set; }
        public string Libelle { get; set; }

        public ValeurLibelle()
        {

        }

        public ValeurLibelle(int pValeur, string pLibelle)
        {
            this.Valeur = pValeur;
            this.Libelle = pLibelle;
        }
    }

    public class SaisirPaiementPieceVenteModel
    {
        public int IDPaiement { get; set; }
        public DateTime Date { get; set; }
        public string Montant { get; set; }
        public TypePaiement TypePaiement { get; set; }

        public SaisirPaiementPieceVenteModel()
        {
            this.Date = DateTime.Now;
        }

        public SaisirPaiementPieceVenteModel(PieceVentePaiement pPaiement)
        {
            this.IDPaiement = pPaiement.ID;
            this.Date = pPaiement.Date;
            this.Montant = pPaiement.Montant.ToString().Replace(',', '.');
            this.TypePaiement = pPaiement.TypePaiement;
        }

        public SaisirPaiementPieceVenteModel(int pIDPaiement, DateTime pDate, float pMontant, TypePaiement pTypePaiement)
        {
            this.IDPaiement = pIDPaiement;
            this.Date = pDate;
            this.Montant = pMontant.ToString().Replace(',', '.');
            this.TypePaiement = pTypePaiement;
        }
    }

    public class DashboardModel
    {
        public int NombreClients { get; set; }
        public float ChiffreAffaireAnnuel { get; set; }
        public float BeneficeMoisEnCours { get; set; }
        public float BeneficeAnnuel { get; set; }
    }

    public class RendezVousModel
    {
        public int ID { get; set; }
        [Display(Name = "Titre")]
        public string Sujet { get; set; }
        [Display(Name = "Début")]
        public DateTime? Debut { get; set; }
        public string DateDebut { get; set; }
        public string HeureDebut { get; set; }
        [Display(Name = "Fin")]
        public DateTime? Fin { get; set; }
        public string DateFin { get; set; }
        public string HeureFin { get; set; }
        public int Duree { get; set; }
        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }

        public RendezVousModel()
        {
            this.ID = 0;
        }

        public RendezVousModel(RendezVous pRendezVous)
        {
            if (pRendezVous != null)
            {
                this.ID = pRendezVous.ID;
                this.Sujet = pRendezVous.Sujet;
                this.Debut = pRendezVous.DateHeureDebut;
                this.Fin = pRendezVous.DateHeureFin;
                if (pRendezVous.DateHeureDebut != null)
                {
                    this.DateDebut = pRendezVous.DateHeureDebut.Value.Date.ToShortDateString();
                    this.HeureDebut = pRendezVous.DateHeureDebut.Value.ToShortTimeString();
                }
                if (pRendezVous.DateHeureFin != null)
                {
                    this.DateFin = pRendezVous.DateHeureFin.Value.Date.ToShortDateString();
                    this.HeureFin = pRendezVous.DateHeureFin.Value.ToShortTimeString();
                }
                if (pRendezVous.Duree != null)
                    this.Duree = pRendezVous.Duree.Value;
                this.Commentaire = pRendezVous.Commentaire;
            }
        }
    }

    public class RendezVousCalendarModel
    {
        public string Text { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }

        public RendezVousCalendarModel()
        {

        }
        public RendezVousCalendarModel(RendezVous rendezVous)
        {
            if (rendezVous.DateHeureDebut != null)
                this.DateDebut = rendezVous.DateHeureDebut.Value;
            if (rendezVous.DateHeureFin != null)
                this.DateFin = rendezVous.DateHeureFin.Value;
            this.Text = rendezVous.Sujet;
        }
    }

    public class AnnulerFactureModel
    {
        public int IDFacture { get; set; }
        public int? Numero { get; set; }
        [Display(Name = "Motif")]
        public string CommentaireAnnulation { get; set; }

        public AnnulerFactureModel()
        {

        }

        public AnnulerFactureModel(PieceVente pFacture)
        {
            this.IDFacture = pFacture.ID;
            this.Numero = pFacture.NumFacture;
        }
    }

    public class FinancierModel
    {
        [Display(Name = "Début")]
        public string DateDebut { get; set; }
        [Display(Name = "Fin")]
        public string DateFin { get; set; }
        public List<FactureFinancierModel> Factures { get; set; }

        public FinancierModel()
        {
            this.Factures = new List<FactureFinancierModel>();
        }
        public FinancierModel(string pDateDebut, string pDateFin, List<FactureFinancierModel> pFactures)
        {
            this.DateDebut = pDateDebut;
            this.DateFin = pDateFin;
            this.Factures = pFactures;
        }
    }

    public class FactureFinancierModel
    {
        public int ID { get; set; }
        [Display(Name = "Numéro")]
        public int? Numero { get; set; }
        [Display(Name = "Client")]
        public ClientModel Client { get; set; }
        [Display(Name = "Voiture")]
        public VoitureModel Voiture { get; set; }
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Client TTC")]
        public double ClientTTC { get; set; }
        [Display(Name = "Garage TTC")]
        public double GarageTTC { get; set; }
        [Display(Name = "Main d'oeuvre")]
        public double MainDOeuvre { get; set; }
        [Display(Name = "Bénéfice")]
        public double Benefice { get; set; }
        [Display(Name = "Reste à payer")]
        public double ResteAPayer { get; set; }

        public FactureFinancierModel()
        {
        }
        public FactureFinancierModel(PieceVente pFacture)
        {
            this.ID = pFacture.ID;
            this.Numero = pFacture.NumFacture;
            this.Client = new ClientModel(pFacture.Client);
            this.Voiture = new VoitureModel(pFacture.Voiture);
            this.Date = DateTime.Now;
            if (pFacture.DateFacture != null)
                this.Date = pFacture.DateFacture.Value;
            this.MainDOeuvre = pFacture.MainDOeuvreMontantHoraire * pFacture.MainDOeuvreDuree;
            this.ClientTTC = pFacture.TotalTTC + this.MainDOeuvre;
            this.GarageTTC = pFacture.Lignes.Sum(l => l.PrixGarageTTC);
            this.Benefice = this.ClientTTC - this.GarageTTC;
            this.ResteAPayer = this.ClientTTC - pFacture.Paiements.Sum(s => s.Montant);
        }
    }

    public class DisplayClient
    {
        public string Nom { get; set; }
        public string Telephone { get; set; }
        public DisplayClient()
        {

        }
        public DisplayClient(Client pClient)
        {
            this.Nom = pClient.Prenom + " " + pClient.Nom;
            this.Telephone = pClient.Telephone;
        }
    }

    public class SearchPieceVenteModel
    {
        [Display(Name = "Début")]
        public DateTime DateDebut { get; set; }
        [Display(Name = "Fin")]
        public DateTime DateFin { get; set; }
        [Display(Name = "Client")]
        public int? ClientID { get; set; }
        public string TypeListe { get; set; }
    }

    public class SearchPieceVenteWithPiecesVenteModel
    {
        public SearchPieceVenteModel Search { get; set; }
        public IQueryable<PieceVente> PiecesVente { get; set; }

        public SearchPieceVenteWithPiecesVenteModel()
        {

        }
    }

    public class FichierComptable
    {
        public string Date { get; set; }
        public string NumFacture { get; set; }
        public string Compte { get; set; }
        public string CodeTVA { get; set; }
        public string Fournisseur { get; set; }
        public string CaseVide1 { get; set; }
        public string CaseVide2 { get; set; }
        public string Debit { get; set; }
        public string Credit { get; set; }
        public FichierComptable()
        {

        }
        public FichierComptable(string pDate, string pNumFacture, string pCompte, string pCodeTVA, string pFournisseur, string pDebit, string pCredit)
        {
            this.Date = pDate;
            this.NumFacture = pNumFacture;
            this.CodeTVA = pCodeTVA;
            this.Compte = pCompte;
            this.Fournisseur = pFournisseur;
            this.Debit = pDebit;
            this.Credit = pCredit;
        }
    }
}
