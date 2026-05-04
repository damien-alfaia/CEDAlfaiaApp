using DevExpress.Web.Mvc;
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
using DevExpress.DataAccess.EntityFramework;

namespace CEDAlfaiaApp.Controllers
{
    public class ClientsController : CEDAlfaiaAppControllerBase
    {
        /*[HttpGet]
        public ActionResult Liste()
        {
            var clients = CEDAlfaiaAppWebService.GetService().GarageService.GetClients();
            ViewBag.UserID = this.UserID.Value;
            return View(clients.Select(c => new ClientModel(c)).ToList());
        }*/

        [HttpGet]
        public ActionResult Liste()
        {
            ViewBag.UserID = this.UserID.Value;
            return View(CEDAlfaiaAppWebService.GetService().GarageService.Clients());
        }


        [HttpGet]
        public ActionResult ListeProspects()
        {
            ViewBag.UserID = this.UserID.Value;
            return View(CEDAlfaiaAppWebService.GetService().GarageService.Prospects());
        }

        [HttpGet]
        public ActionResult Test()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ListeSupprimes()
        {
            var clients = CEDAlfaiaAppWebService.GetService().GarageService.GetClientsSupprimes();
            ViewBag.UserID = this.UserID.Value;
            return View(clients.Select(c => new ClientModel(c)).ToList());
        }

        [HttpGet]
        public ActionResult Saisir(string idClient)
        {
            int id = 0;
            int.TryParse(idClient, out id);
            if (id > 0)
            {
                Client client = CEDAlfaiaAppWebService.GetService().GarageService.GetClient(id);
                ViewBag.Marques = new JavaScriptSerializer().Serialize(CEDAlfaiaAppWebService.GetService().GarageService.GetAllMarques().Select(m => new MarqueModel(m)).OrderBy(m => m.Libelle).ToList());

                return View(new ClientModel(client));
            }
            return View();
        }

        [HttpPost]
        public ActionResult Saisir(ClientModel pClient)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (pClient != null)
            {
                int id = pClient.ID != null ? pClient.ID.Value : 0;
                var userCreated = CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateClient(id, pClient.Nom, pClient.Prenom, pClient.Telephone, pClient.Email, pClient.Adresse.Ligne1, pClient.Adresse.Ligne2, pClient.Adresse.Ligne3, pClient.Adresse.CodePostal, pClient.Adresse.Ville, pClient.Adresse.Pays, pClient.Complements, pClient.Remise, pClient.IsProspect);
                if (userCreated != null && userCreated.IsProspect)
                    return RedirectToAction("ListeProspects");
                return RedirectToAction("Liste");
            }

            return View();
        }
        [HttpGet]
        public void GenererCodes()
        {
            CEDAlfaiaAppWebService.GetService().GarageService.GenererProspects();
            CEDAlfaiaAppWebService.GetService().GarageService.GenererCodesClients();
        }
        [HttpGet]
        public ActionResult SaisirVoiture(string idClient, string idVoiture)
        {
            if (idVoiture == null)
            {
                int id = 0;
                int.TryParse(idClient, out id);
                if (id > 0)
                {
                    Client client = CEDAlfaiaAppWebService.GetService().GarageService.GetClient(id);
                    if (client != null)
                    {
                        ViewBag.NomClient = client.Nom + " " + client.Prenom;
                        return View(new VoitureModel(id));
                    }
                }
            }
            else
            {
                int id = 0;
                if (int.TryParse(idVoiture, out id))
                {
                    Voiture voitureFound = CEDAlfaiaAppWebService.GetService().GarageService.GetVoiture(id);
                    ViewBag.NomClient = voitureFound.Client.Nom + " " + voitureFound.Client.Prenom;
                    if (voitureFound != null)
                        return View(new VoitureModel(voitureFound));
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaisirVoiture(VoitureModel pVoiture)
        {
            if (!ModelState.IsValid)
            {
                return View(pVoiture);
            }

            try
            {
                if (pVoiture.ID > 0)
                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateVoitureClient(pVoiture.IDModele, pVoiture.Immatriculation, pVoiture.IDClient, pVoiture.ID);
                else
                    CEDAlfaiaAppWebService.GetService().GarageService.AddUpdateVoitureClient(pVoiture.IDModele, pVoiture.Immatriculation, pVoiture.IDClient);

                var clientFound = CEDAlfaiaAppWebService.GetService().GarageService.GetClient(pVoiture.IDClient);
                if (clientFound != null && clientFound.IsProspect)
                    return RedirectToAction("ListeProspects");
                return RedirectToAction("Liste");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(pVoiture);
            }
        }

        [HttpPost]
        public string PasserEnClient(int? idProspect)
        {
            try
            {
                if (idProspect != null)
                {
                    CEDAlfaiaAppWebService.GetService().GarageService.PasserProspectEnClient(idProspect.Value);
                    return "OK";
                }
                else
                {
                    return "KO";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string SupprimerVoiture(string idVoiture)
        {
            try
            {

                int id = 0;
                int.TryParse(idVoiture, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.DeleteVoitureClient(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public string SupprimerClient(string idClient)
        {
            try
            {
                int id = 0;
                int.TryParse(idClient, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.DeleteClient(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        [HttpPost]
        public string ReactiverClient(string idClient)
        {
            try
            {
                int id = 0;
                int.TryParse(idClient, out id);
                CEDAlfaiaAppWebService.GetService().GarageService.ReactiverClient(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        CEDAlfaiaApp.DAL.CEDAlfaiaAppContext db = new CEDAlfaiaApp.DAL.CEDAlfaiaAppContext();

        [ValidateInput(false)]
        public ActionResult GridViewListeClientPartial()
        {
            var model = db.Clients;
            return PartialView("_GridViewListeClientPartial", model.ToList());
        }

        [ValidateInput(false)]
        public ActionResult GridViewTestClientPartial()
        {
            return PartialView("_FactureReportPartial");
        }
        
    }
}