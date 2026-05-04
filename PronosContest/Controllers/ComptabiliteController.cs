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
    public class ComptabiliteController : CEDAlfaiaAppControllerBase
    {
        [HttpGet]
        public ActionResult BalanceAgee()
        {
            return View(CEDAlfaiaAppWebService.GetService().GarageService.BalanceAgee());
        }

        
    }
}