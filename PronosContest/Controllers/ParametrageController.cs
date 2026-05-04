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
using CEDAlfaiaApp.DAL.Parametrage;

namespace CEDAlfaiaApp.Controllers
{
    public class ParametrageController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Saisir()
        {
            Parametrage param = new Parametrage();
            if (this.UtilisateurEnCours != null && this.UtilisateurEnCours.Entreprise != null)
                param = this.UtilisateurEnCours.Entreprise.Parametrage;
            return View(new ParametrageModel(param));
        }

        [HttpPost]
        public ActionResult Saisir(ParametrageModel pParametrage)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (pParametrage != null)
            {
                int idEntreprise = (this.UtilisateurEnCours.EntrepriseID != null ? this.UtilisateurEnCours.EntrepriseID.Value : 0);
                string imageBase64 = null;
                if (pParametrage.Logo != null)
                    imageBase64 = pParametrage.Logo.Split(',').Last();
                string enteteBase64 = null;
                if (pParametrage.Entete != null)
                    enteteBase64 = pParametrage.Entete.Split(',').Last();
                CEDAlfaiaAppWebService.GetService().ParametrageService.AddUpdateParametrage(pParametrage.ID, idEntreprise, pParametrage.LibelleEntreprise, pParametrage.Adresse.Ligne1, pParametrage.Adresse.Ligne2, pParametrage.Adresse.Ligne3, pParametrage.Adresse.CodePostal, pParametrage.Adresse.Ville, pParametrage.Adresse.Pays, pParametrage.Logo != null ? Convert.FromBase64String(imageBase64) : null, pParametrage.TauxTVA, pParametrage.EmailComptable, pParametrage.Telephone, pParametrage.Portable, pParametrage.Email, pParametrage.IsSavePieceVente, pParametrage.MainDOeuvreMontantHoraire, pParametrage.ServeurSMTP, pParametrage.PortSMTP, pParametrage.IsSSL, pParametrage.ProtocoleSMTP, pParametrage.EmailEnvoi, pParametrage.EmailSMTP, pParametrage.PasswordSMTP, pParametrage.Entete != null ? Convert.FromBase64String(enteteBase64) : null, pParametrage.SIRET, pParametrage.CodeAPE, pParametrage.TVAIntraCommunautaire, pParametrage.LibelleBasDePage);
                return RedirectToAction("Index","Home");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Salaries()
        {
            return View();
        }
    }
}