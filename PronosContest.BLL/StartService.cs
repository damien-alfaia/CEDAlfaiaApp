using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Authentification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CEDAlfaiaApp.BLL
{
	public class StartService
	{
		private CEDAlfaiaAppContext _CEDAlfaiaAppContextDatabase;

		internal StartService(CEDAlfaiaAppContext pCEDAlfaiaAppContextDatabase)
		{
			_CEDAlfaiaAppContextDatabase = pCEDAlfaiaAppContextDatabase;
		}
		public void Init()
		{
            var superUtilisateur = _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Where(u => u.Email == "damien_alfaia@live.fr").FirstOrDefault();
            if (superUtilisateur == null)
                _CEDAlfaiaAppContextDatabase.CompteUtilisateurs.Add(new CompteUtilisateur("damien_alfaia@live.fr", "Alfaia230989", "Alfaia", "Damien", new DAL.Shared.Adresse(), CompteUtilisateurRole.SuperAdmin));
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
    }
}
