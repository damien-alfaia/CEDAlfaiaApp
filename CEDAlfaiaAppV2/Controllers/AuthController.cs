using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.Core;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaAppV2.Models;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Controllers
{
    [AllowAnonymous]
    public class AuthController : CEDAlfaiaAppControllerBase
	{
        [HttpGet]
        public ActionResult LogIn(string returnUrl, string pEmail = "")
		{            
            var model = new LogInModel {
                Email = pEmail,
				ReturnUrl = returnUrl
			};
            return View(model);
        }

        [HttpPost]
		public ActionResult LogIn(LogInModel pModel)
		{
			if (!ModelState.IsValid)
				return View();

            var user = CEDAlfaiaAppWebService.GetService().AuthenticationService.Connexion(pModel.Email, pModel.Password);
            if (user != null)
            {
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, user.Prenom != null ? user.Prenom : string.Empty),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Surname, user.Nom != null ? user.Nom : string.Empty),
                    new Claim(ClaimTypes.Sid, user.ID.ToString()),
                    new Claim(ClaimTypes.Role, ((int)user.Role).ToString())
                }, "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;
                
                authManager.SignIn(identity);

                CEDAlfaiaAppHttpContext.UserID = user.ID;
                if (user.Entreprise != null && user.Entreprise.Parametrage != null)
                    CEDAlfaiaAppHttpContext.TauxTVA = user.Entreprise.Parametrage.TVA;


                return Redirect(GetRedirectUrl(pModel.ReturnUrl));
            }
            
			// user authN failed
			ModelState.AddModelError("", "Vos identifiants sont incorrects !");
			return View(pModel);
		}

        [HttpGet]
        public ActionResult LogOut()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut();

			return RedirectToAction("LogIn");
        }

        private string GetRedirectUrl(string returnUrl)
		{
			if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
			{
				return Url.Action("index", "home");
			}

			return returnUrl;
		}

		[HttpGet]
		public ActionResult Inscription()
		{
			var model = new InscriptionModel();
			return View(model);
		}

		[HttpPost]
		public ActionResult Inscription(InscriptionModel pModel)
		{
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (pModel != null)
            {
                var userCreated = CEDAlfaiaAppWebService.GetService().AuthenticationService.Inscrire(pModel.Email, pModel.Password, pModel.Nom, pModel.Prenom, pModel.Adresse, pModel.Role);
                return RedirectToAction("LogIn", new { returnUrl = "", pEmail = userCreated.Email });
            }

            return View();
		}
	}
}