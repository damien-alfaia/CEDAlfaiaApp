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
    public class EntrepriseController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Liste()
        {
            var entreprises = CEDAlfaiaAppWebService.GetService().ParametrageService.GetEntreprises();
            ViewBag.UserID = this.UserID.Value;
            return View(entreprises.Select(f => new EntrepriseModel(f)).ToList());
        }

        [HttpGet]
        public ActionResult Saisir(string idEntreprise)
        {
            int id = 0;
            int.TryParse(idEntreprise, out id);
            if (id > 0)
            {
                Entreprise entreprise = CEDAlfaiaAppWebService.GetService().ParametrageService.GetEntreprise(id);
                return View(new EntrepriseModel(entreprise));
            }
            return View(new EntrepriseModel());
        }

        [HttpPost]
        public ActionResult Saisir(EntrepriseModel pEntreprise)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (pEntreprise != null)
            {
                var entrepriseCreated = CEDAlfaiaAppWebService.GetService().ParametrageService.AddUpdateEntreprise(pEntreprise.ID, pEntreprise.Nom, pEntreprise.Adresse.Ligne1, pEntreprise.Adresse.Ligne2, pEntreprise.Adresse.Ligne3, pEntreprise.Adresse.CodePostal, pEntreprise.Adresse.Ville, pEntreprise.Adresse.Pays, pEntreprise.Commentaire, pEntreprise.Siren, pEntreprise.Siret, pEntreprise.IsActif);
                return RedirectToAction("Liste");
            }

            return View();
        }
    }
}