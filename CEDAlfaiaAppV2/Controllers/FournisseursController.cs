using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaAppV2.Models;
using System.Linq;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Controllers
{
    public class FournisseursController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Liste()
        {
            var fournisseurs = CEDAlfaiaAppWebService.GetService().GarageService.GetFournisseurs();
            ViewBag.UserID = this.UserID.Value;
            return View(fournisseurs.Select(f => new FournisseurModel(f)).ToList());
        }

        [HttpGet]
        public ActionResult Saisir(string idFournisseur)
        {
            int id = 0;
            int.TryParse(idFournisseur, out id);
            if (id > 0)
            {
                Fournisseur fournisseur = CEDAlfaiaAppWebService.GetService().GarageService.GetFournisseur(id);
                return View(new FournisseurModel(fournisseur));
            }
            return View(new FournisseurModel());
        }

        [HttpPost]
        public ActionResult Saisir(FournisseurModel pFournisseur)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (pFournisseur != null)
            {
                var userCreated = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateFournisseur(pFournisseur.ID, pFournisseur.Nom, pFournisseur.Adresse.Ligne1, pFournisseur.Adresse.Ligne2, pFournisseur.Adresse.Ligne3, pFournisseur.Adresse.CodePostal, pFournisseur.Adresse.Ville, pFournisseur.Adresse.Pays, pFournisseur.Commentaire);
                return RedirectToAction("Liste");
            }

            return View();
        }
    }
}