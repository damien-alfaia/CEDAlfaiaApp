using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Parametrage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace CEDAlfaiaApp.BLL
{
    public class ParametrageService
    {
        private CEDAlfaiaAppContext _CEDAlfaiaAppContextDatabase;

        internal ParametrageService(CEDAlfaiaAppContext pCEDAlfaiaAppContextDatabase)
        {
            _CEDAlfaiaAppContextDatabase = pCEDAlfaiaAppContextDatabase;
        }

        #region Entreprises
        public List<Entreprise> GetEntreprises()
        {
            return _CEDAlfaiaAppContextDatabase.Entreprises.ToList();
        }
        public Entreprise GetEntreprise(int id)
        {
            return _CEDAlfaiaAppContextDatabase.Entreprises.Where(c => c.ID == id).FirstOrDefault();
        }
        public Entreprise AddUpdateEntreprise(int pID, string pNom, string pLigne1, string pLigne2, string pLigne3, string pCodePostal, string pVille, string pPays, string pCommentaire, string pSiren, string pSiret, bool pIsActif)
        {
            Entreprise entrepriseResult = new Entreprise();
            if (pID > 0)
                entrepriseResult = _CEDAlfaiaAppContextDatabase.Entreprises.Where(c => c.ID == pID).FirstOrDefault();

            entrepriseResult.Nom = pNom;
            if (entrepriseResult.Adresse == null)
                entrepriseResult.Adresse = new DAL.Shared.Adresse();
            entrepriseResult.Adresse.Ligne1 = pLigne1;
            entrepriseResult.Adresse.Ligne2 = pLigne2;
            entrepriseResult.Adresse.Ligne3 = pLigne3;
            entrepriseResult.Adresse.CodePostal = pCodePostal;
            entrepriseResult.Adresse.Ville = pVille;
            entrepriseResult.Adresse.Pays = pPays;
            entrepriseResult.Commentaire = pCommentaire;
            entrepriseResult.IsActif = pIsActif;
            entrepriseResult.Siren = pSiren;
            entrepriseResult.Siret = pSiret;
            entrepriseResult.DateCreation = DateTime.Now;
            if (pID == 0)
                entrepriseResult = _CEDAlfaiaAppContextDatabase.Entreprises.Add(entrepriseResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return entrepriseResult;
        }
        public void DeleteEntreprise(int pID)
        {
            Entreprise entrepriseResult = new Entreprise();
            if (pID > 0)
                entrepriseResult = _CEDAlfaiaAppContextDatabase.Entreprises.Where(c => c.ID == pID).FirstOrDefault();

            _CEDAlfaiaAppContextDatabase.Entreprises.Remove(entrepriseResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        #endregion
        public Parametrage AddUpdateParametrage(int pID, int pEntrepriseID, string pNomEntreprise, string pLigne1, string pLigne2, string pLigne3, string pCodePostal, string pVille, string pPays, byte[] pLogo, float pTauxTVA, string pEmailComptable, string pTelephoneEntreprise, string pPortableEntreprise, string pEmail, bool pIsSavePieceVente, float pMainDOeuvreMontantHoraire, string pServeurSMTP, int? pPortSMTP, bool pIsSSL, string pProtocoleSMTP, string pEmailEnvoi, string pEmailSMTP, string pPasswordSMTP, byte[] pEntete, string pSiret, string pCodeAPE, string pTVAIntraCommunautaire, string pLibelleBasDePage, float? pObjectifAnnuel)
        {
            Parametrage parametrageResult = new Parametrage();
            Entreprise entrepriseResult = new Entreprise();
            if (pEntrepriseID > 0)
                entrepriseResult = _CEDAlfaiaAppContextDatabase.Entreprises.Where(c => c.ID == pEntrepriseID).FirstOrDefault();
            if (entrepriseResult.Parametrage != null)
                parametrageResult = entrepriseResult.Parametrage;

            parametrageResult.NomEntreprise = pNomEntreprise;
            if (parametrageResult.Adresse == null)
                parametrageResult.Adresse = new DAL.Shared.Adresse();
            parametrageResult.Adresse.Ligne1 = pLigne1;
            parametrageResult.Adresse.Ligne2 = pLigne2;
            parametrageResult.Adresse.Ligne3 = pLigne3;
            parametrageResult.Adresse.CodePostal = pCodePostal;
            parametrageResult.Adresse.Ville = pVille;
            parametrageResult.Adresse.Pays = pPays;
            parametrageResult.Logo = pLogo;
            parametrageResult.EmailComptable = pEmailComptable;
            parametrageResult.TVA = pTauxTVA;
            parametrageResult.EmailEntreprise = pEmail;
            parametrageResult.TelephoneEntreprise = pTelephoneEntreprise;
            parametrageResult.PortableEntreprise = pPortableEntreprise;
            parametrageResult.IsSavePieceDeVente = pIsSavePieceVente;
            parametrageResult.MainDOeuvreMontantHoraire = pMainDOeuvreMontantHoraire;

            parametrageResult.IsSSL = pIsSSL;
            parametrageResult.ServeurSMTP = pServeurSMTP;
            parametrageResult.PortSMTP = pPortSMTP;
            parametrageResult.ProtocoleSMTP = pProtocoleSMTP;
            parametrageResult.EmailEnvoiSMTP = pEmailEnvoi;
            parametrageResult.EmailSMTP = pEmailSMTP;
            parametrageResult.PasswordSMTP = pPasswordSMTP;

            parametrageResult.Entete = pEntete;

            parametrageResult.ObjectifAnnuel = pObjectifAnnuel;

            parametrageResult.CodeAPE = pCodeAPE;
            parametrageResult.TVAIntraCommunautaire = pTVAIntraCommunautaire;
            parametrageResult.SIRET = pSiret;
            parametrageResult.LibelleBasDePage = pLibelleBasDePage;

            if (parametrageResult.ID == 0)
                parametrageResult = _CEDAlfaiaAppContextDatabase.Parametrages.Add(parametrageResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();

            entrepriseResult.Parametrage = parametrageResult;
            _CEDAlfaiaAppContextDatabase.SaveChanges();

            return entrepriseResult.Parametrage;
        }

        public Parametrage GetParametrageUtilisateur(int idUtilisateur)
        {
            CompteUtilisateur userFound = _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Where(u => u.ID == idUtilisateur).FirstOrDefault();
            if (userFound != null && userFound.Entreprise != null && userFound.Entreprise.Parametrage != null)
                return userFound.Entreprise.Parametrage;
            return null;
        }
    }
}
