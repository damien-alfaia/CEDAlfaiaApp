using CEDAlfaiaApp.BLL;
using CEDAlfaiaAppV2.Code;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Controllers
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