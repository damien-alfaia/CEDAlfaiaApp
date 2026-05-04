using CEDAlfaiaApp.Core;
using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace CEDAlfaiaApp.BLL
{
    public class AuthentificationService
    {
        private CEDAlfaiaAppContext _CEDAlfaiaAppContextDatabase;

        internal AuthentificationService(CEDAlfaiaAppContext pCEDAlfaiaAppContextDatabase)
        {
            _CEDAlfaiaAppContextDatabase = pCEDAlfaiaAppContextDatabase;
        }
        public IEnumerable<CompteUtilisateur> GetAllUser()
        {
            return _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.ToList();
        }

        public CompteUtilisateur GetUserById(int pId)
        {
            return _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Where(cu => cu.ID == pId).FirstOrDefault();
        }
        public CompteUtilisateur Connexion(string pEmail, string pPassword)
        {
            byte[] passwordHashed = pPassword.ToPasswordHash();
            return _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Where(cu => cu.Email == pEmail && cu.Password == passwordHashed).FirstOrDefault();
        }
        public CompteUtilisateur Inscrire(string pEmail, string pPassword, string pNom, string pPrenom, Adresse pAdresse, CompteUtilisateurRole pRole)
        {
            CompteUtilisateur newUser = new CompteUtilisateur(pEmail, pPassword, pNom, pPrenom, pAdresse, pRole);
            using (TransactionScope scope = new TransactionScope())
            {
                newUser = _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Add(newUser);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
                scope.Complete();
            }
            return newUser;
        }
        public CompteUtilisateur AddUpdateUtilisateur(int pID, string pNom, string pPrenom, string pEmail, string pPassword, string pLigne1, string pLigne2, string pLigne3, string pCodePostal, string pVille, string pPays, string pCommentaire, CompteUtilisateurRole pRole, int? idEntreprise)
        {
            CompteUtilisateur userResult = new CompteUtilisateur();
            if (pID > 0)
                userResult = _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Where(c => c.ID == pID).FirstOrDefault();

            userResult.Nom = pNom;
            userResult.Prenom = pPrenom;
            if (userResult.Adresse == null)
                userResult.Adresse = new DAL.Shared.Adresse();
            userResult.Adresse.Ligne1 = pLigne1;
            userResult.Adresse.Ligne2 = pLigne2;
            userResult.Adresse.Ligne3 = pLigne3;
            userResult.Adresse.CodePostal = pCodePostal;
            userResult.Adresse.Ville = pVille;
            userResult.Adresse.Pays = pPays;
            userResult.EntrepriseID = idEntreprise;
            userResult.Email = pEmail;
            userResult.Password = pPassword.ToPasswordHash();
            userResult.Role = pRole;

            if (pID == 0)
                userResult = _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Add(userResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return userResult;
        }
        public void DeleteUtilisateur(int pID)
        {
            CompteUtilisateur userResult = new CompteUtilisateur();
            if (pID > 0)
                userResult = _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Where(c => c.ID == pID).FirstOrDefault();

            _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Remove(userResult);
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }

    }
}
