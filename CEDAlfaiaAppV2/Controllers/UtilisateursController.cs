using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaAppV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Controllers
{
    public class UtilisateursController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Liste()
        {
            var utilisateurs = CEDAlfaiaAppWebService.GetService().AuthenticationService.GetAllUser();
            ViewBag.UserID = this.UserID.Value;
            return View(utilisateurs.Select(u => new UtilisateurModel(u)).ToList());
        }

        [HttpGet]
        public ActionResult Saisir(string idUtilisateur)
        {
            var entreprisesFound = CEDAlfaiaAppWebService.GetService().ParametrageService.GetEntreprises();
            List<SelectListItem> listItems = new List<SelectListItem>();
            if (entreprisesFound != null)
            {
                listItems = entreprisesFound.Select(ent => new SelectListItem()
                {
                    Text = ent.Nom,
                    Value = ent.ID.ToString()
                }).ToList();
            }
            ViewBag.EntreprisesListItems = listItems;

            int id = 0;
            int.TryParse(idUtilisateur, out id);
            if (id > 0)
            {
                CompteUtilisateur user = CEDAlfaiaAppWebService.GetService().AuthenticationService.GetUserById(id);
                return View(new UtilisateurModel(user));
            }
            return View(new UtilisateurModel());
        }

        [HttpPost]
        public ActionResult Saisir(UtilisateurModel pUtilisateur)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (pUtilisateur != null)
            {
                var entrepriseCreated = CEDAlfaiaAppWebService.GetService().AuthenticationService.AddUpdateUtilisateur(pUtilisateur.ID, pUtilisateur.Nom, pUtilisateur.Prenom, pUtilisateur.AdresseMail, pUtilisateur.MotDePasse, pUtilisateur.Adresse.Ligne1, pUtilisateur.Adresse.Ligne2, pUtilisateur.Adresse.Ligne3, pUtilisateur.Adresse.CodePostal, pUtilisateur.Adresse.Ville, pUtilisateur.Adresse.Pays, pUtilisateur.Commentaire, (CompteUtilisateurRole)pUtilisateur.Role, pUtilisateur.Entreprise.ID);
                return RedirectToAction("Liste");
            }

            return View();
        }

        [HttpPost]
        public string SupprimerUtilisateur(string idUtilisateur)
        {
            try
            {

                int id = 0;
                int.TryParse(idUtilisateur, out id);
                CEDAlfaiaAppWebService.GetService().AuthenticationService.DeleteUtilisateur(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}