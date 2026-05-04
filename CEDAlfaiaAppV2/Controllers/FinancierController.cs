using CEDAlfaiaApp.BLL;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaAppV2.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Controllers
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