using CEDAlfaiaApp.BLL;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaAppV2.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Controllers
{
    public class WidgetsController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Liste()
        {
            return View(CEDAlfaiaAppWebService.GetService().WidgetService.GetWidgetsUtilisateurByUtilisateur(this.UserID.Value));
        }

        [HttpGet]
        public ActionResult Widgets()
        {
            CEDAlfaiaAppWebService.GetService().WidgetService.InitWidgets();

            return View(CEDAlfaiaAppWebService.GetService().WidgetService.GetWidgetsUtilisateurByUtilisateur(this.UserID.Value).ToList());
        }

        [HttpGet]
        public ActionResult Saisir(int? idWidget)
        {
            WidgetUtilisateurModel widget = new WidgetUtilisateurModel();
            if (this.UtilisateurEnCours != null)
                widget.CompteUtilisateurID = this.UtilisateurEnCours.ID;

            if (idWidget != null)
                widget = new WidgetUtilisateurModel(CEDAlfaiaAppWebService.GetService().WidgetService.GetWidgetsUtilisateurByID(idWidget.Value));
            return View(widget);
        }

        [HttpPost]
        public ActionResult Saisir(WidgetUtilisateurModel pWidgetUtilisateur)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (pWidgetUtilisateur != null)
            {
                CEDAlfaiaAppWebService.GetService().WidgetService.AddUpdateWidgetUtilisateur(pWidgetUtilisateur.ID, pWidgetUtilisateur.CompteUtilisateurID.Value, pWidgetUtilisateur.WidgetID.Value, pWidgetUtilisateur.Titre, pWidgetUtilisateur.HtmlIndex, pWidgetUtilisateur.HtmlLeft, pWidgetUtilisateur.HtmlTop, pWidgetUtilisateur.HtmlOwnerZoneUID, pWidgetUtilisateur.DateDebut, pWidgetUtilisateur.DateFin, pWidgetUtilisateur.IsAnneeEnCours, pWidgetUtilisateur.IsMoisEnCours);
                return RedirectToAction("Widgets", "Widgets");
            }

            return View();
        }

        [HttpPost]
        public string SaveWidgetUtilisateur(string nameWidget, string zone, string top, string left)
        {
            if (nameWidget.Split('_').Count() > 0)
            {
                int id = Convert.ToInt32(nameWidget.Split('_').Last());
                CEDAlfaiaAppWebService.GetService().WidgetService.SavePositionWidget(id, zone, top, left);
                return "OK";
            }
            return "KO";
        }
        [HttpPost]
        public string ResetWidgetUtilisateur(string nameWidget)
        {
            if (nameWidget.Split('_').Count() > 0)
            {
                int id = Convert.ToInt32(nameWidget.Split('_').Last());
                CEDAlfaiaAppWebService.GetService().WidgetService.SavePositionWidget(id, "", "", "");
                return "OK";
            }
            return "KO";
        }

        [HttpPost]
        public string SupprimerWidget(string idWidget)
        {
            try
            {
                int id = 0;
                int.TryParse(idWidget, out id);
                CEDAlfaiaAppWebService.GetService().WidgetService.DeleteWidgetUtilisateur(id);
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}