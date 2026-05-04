using CEDAlfaiaApp.DAL.Parametrage;
using CEDAlfaiaApp.DAL.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System;
using CEDAlfaiaApp.DAL.Authentification;

namespace CEDAlfaiaAppV2.Models
{

    public class EntrepriseModel
    {
        public int ID { get; set; }
        [Display(Name = "Nom")]
        public string Nom { get; set; }
        [Display(Name = "SIREN")]
        public string Siren { get; set; }
        [Display(Name = "SIRET")]
        public string Siret { get; set; }
        [Display(Name = "Actif")]
        public bool IsActif { get; set; }
        [Display(Name = "Adresse")]
        public AdresseModel Adresse { get; set; }
        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }

        public EntrepriseModel()
        {
            this.ID = 0;
            this.Nom = "";
            this.Siren = "";
            this.Siret = "";
            this.IsActif = false;
            this.Adresse = new AdresseModel();
            this.Commentaire = "";
        }
        public EntrepriseModel(Entreprise pEntreprise)
        {
            this.ID = 0;
            if (pEntreprise != null)
            {
                this.ID = pEntreprise.ID;
                this.Nom = pEntreprise.Nom;
                this.Siren = pEntreprise.Siren;
                this.Siret = pEntreprise.Siret;
                this.IsActif = pEntreprise.IsActif;
                this.Adresse = new AdresseModel(pEntreprise.Adresse);
                this.Commentaire = pEntreprise.Commentaire;
            }
        }
    }

    public class UtilisateurModel
    {
        public int ID { get; set; }
        [Display(Name = "Nom")]
        public string Nom { get; set; }
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; }
        [Display(Name = "Email")]
        public string AdresseMail { get; set; }
        [Display(Name = "Adresse")]
        public AdresseModel Adresse { get; set; }
        [Display(Name = "Commentaire")]
        public string Commentaire { get; set; }
        [Display(Name = "Rôle")]
        public int Role { get; set; }
        [Display(Name = "Entreprise")]
        public EntrepriseModel Entreprise { get; set; }

        public UtilisateurModel()
        {
            this.MotDePasse = "0000";
        }
        public UtilisateurModel(CompteUtilisateur pUtilisateur)
        {
            this.ID = pUtilisateur.ID;
            this.Nom = pUtilisateur.Nom;
            this.Prenom = pUtilisateur.Prenom;
            this.MotDePasse = null;
            this.AdresseMail = pUtilisateur.Email;
            this.Adresse = new AdresseModel(pUtilisateur.Adresse);
            this.Role = (int)pUtilisateur.Role;
            this.Entreprise = new EntrepriseModel(pUtilisateur.Entreprise);
        }
    }

    public class ParametrageModel
    {
        public int ID { get; set; }

        [Display(Name = "Nom de l'entreprise")]
        public string LibelleEntreprise { get; set; }
        [Display(Name = "Adresse")]
        public AdresseModel Adresse { get; set; }
        [Display(Name = "Logo")]
        public string Logo { get; set; }
        [Display(Name = "Entête de facture")]
        public string Entete { get; set; }
        [Display(Name = "Taux TVA")]
        public float TauxTVA { get; set; }
        [Display(Name = "Email comptable")]
        public string EmailComptable { get; set; }
        [Display(Name = "Téléphone")]
        public string Telephone { get; set; }
        [Display(Name = "Portable")]
        public string Portable { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Voulez-vous enregistrer les pièces de vente ?")]
        public bool IsSavePieceVente { get; set; }
        [Display(Name = "Tarif horaire de la main d'oeuvre")]
        public float MainDOeuvreMontantHoraire { get; set; }
        [Display(Name = "Siret")]
        public string SIRET { get; set; }
        [Display(Name = "Code APE")]
        public string CodeAPE { get; set; }
        [Display(Name = "TVA IntraCommunataire")]
        public string TVAIntraCommunautaire { get; set; }
        [Display(Name = "Bas de page")]
        public string LibelleBasDePage { get; set; }

        [Display(Name = "Serveur SMTP")]
        public string ServeurSMTP { get; set; }
        [Display(Name = "Port SMTP")]
        public int? PortSMTP { get; set; }
        [Display(Name = "SSL")]
        public bool IsSSL { get; set; }
        [Display(Name = "Protocole SMTP")]
        public string ProtocoleSMTP { get; set; }
        [Display(Name = "Email d'envoi")]
        public string EmailEnvoi { get; set; }
        [Display(Name = "Email SMTP")]
        public string EmailSMTP { get; set; }
        [Display(Name = "Mot de passe SMTP")]
        public string PasswordSMTP { get; set; }

        public ParametrageModel()
        {

        }

        public ParametrageModel(Parametrage pParametrage)
        {
            if (pParametrage != null)
            {
                this.ID = pParametrage.ID;
                this.LibelleEntreprise = pParametrage.NomEntreprise;
                this.Adresse = new AdresseModel(pParametrage.Adresse);
                this.Logo = pParametrage.Logo != null ? Convert.ToBase64String(pParametrage.Logo) : null;
                this.EmailComptable = pParametrage.EmailComptable;
                this.TauxTVA = pParametrage.TVA;
                this.Email = pParametrage.EmailEntreprise;
                this.Telephone = pParametrage.TelephoneEntreprise;
                this.Portable = pParametrage.PortableEntreprise;
                this.IsSavePieceVente = pParametrage.IsSavePieceDeVente;
                this.MainDOeuvreMontantHoraire = pParametrage.MainDOeuvreMontantHoraire;
                this.ServeurSMTP = pParametrage.ServeurSMTP;
                this.PortSMTP = pParametrage.PortSMTP;
                this.ProtocoleSMTP = pParametrage.ProtocoleSMTP;
                this.EmailEnvoi = pParametrage.EmailEnvoiSMTP;
                this.IsSSL = pParametrage.IsSSL;
                this.EmailSMTP = pParametrage.EmailSMTP;
                this.PasswordSMTP = pParametrage.PasswordSMTP;
                this.Entete = pParametrage.Entete != null ? Convert.ToBase64String(pParametrage.Entete) : null;
                this.SIRET = pParametrage.SIRET;
                this.LibelleBasDePage = pParametrage.LibelleBasDePage;
                this.CodeAPE = pParametrage.CodeAPE;
                this.TVAIntraCommunautaire = TVAIntraCommunautaire;
            }
        }
    }
}
