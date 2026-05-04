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
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.IO;
using CEDAlfaiaApp.DAL.Parametrage;

namespace CEDAlfaiaApp.Controllers
{
    public class FinancierController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult Liste(string dateDebut, string dateFin)
        {
            DateTime dateD;
            DateTime dateF;

            if (string.IsNullOrEmpty(dateDebut))
                dateDebut = DateTime.Now.AddDays(-7).ToShortDateString();
            if (string.IsNullOrEmpty(dateFin))
                dateFin = DateTime.Now.AddDays(7).ToShortDateString();

            if (DateTime.TryParse(dateDebut, out dateD) && DateTime.TryParse(dateFin, out dateF))
            {
                var factures = CEDAlfaiaAppWebService.GetService().GarageService.GetFacturesValideesBetweenDates(dateD, dateF);
                return View(new FinancierModel(dateDebut, dateFin, factures.Select(s => new FactureFinancierModel(s)).ToList()));
            }
            else
            {
                return View(new FinancierModel());
            }
        }

        [HttpPost]
        public ActionResult Liste(FinancierModel pModel)
        {
            return Liste(pModel.DateDebut, pModel.DateFin);
        }
    }
}