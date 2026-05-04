using Newtonsoft.Json;
using CEDAlfaiaApp.BLL;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaApp.Core;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaAppV2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CEDAlfaiaApp.DAL.Parametrage;

namespace CEDAlfaiaAppV2.Controllers
{
    public class AgendaController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Agenda ()
        {
            var rendezVous = CEDAlfaiaAppWebService.GetService().GarageService.GetRendezVous();
            return View(rendezVous.Select(rdv => new RendezVousCalendarModel(rdv)).ToList());
        }
        [HttpGet]
        public ActionResult Calendrier(string mois = null, string annee = null)
        {
            int moisRdv = DateTime.Now.Month;
            if (mois != null)
                int.TryParse(mois, out moisRdv);
            int anneeRdv = DateTime.Now.Year;
            if (annee != null)
                int.TryParse(annee, out anneeRdv);

            var rendezVous = CEDAlfaiaAppWebService.GetService().GarageService.GetRendezVous(moisRdv, anneeRdv);
            return View(rendezVous.Select(rdv => new RendezVousCalendarModel(rdv)).ToList());
        }
    }
}