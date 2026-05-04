using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Parametrage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CEDAlfaiaApp.DAL.Salaries;

namespace CEDAlfaiaApp.BLL
{
    public class SalariesService
    {
        private CEDAlfaiaAppContext _CEDAlfaiaAppContextDatabase;

        internal SalariesService(CEDAlfaiaAppContext pCEDAlfaiaAppContextDatabase)
        {
            _CEDAlfaiaAppContextDatabase = pCEDAlfaiaAppContextDatabase;
        }

        public IQueryable<Salarie> Salaries()
        {
            return _CEDAlfaiaAppContextDatabase.Salaries;
        }
        
        public Salarie GetSalarie(int pID)
        {
            return _CEDAlfaiaAppContextDatabase.Salaries.Where(s => s.ID == pID).FirstOrDefault();
        }
        public Salarie AddSalarie(Salarie newSalarie)
        {
            return _CEDAlfaiaAppContextDatabase.Salaries.Add(newSalarie);
        }
        public Salarie UpdateSalarie(Salarie updatedSalarie)
        {
            Salarie salarieFound = _CEDAlfaiaAppContextDatabase.Salaries.Where(s => s.ID == updatedSalarie.ID).FirstOrDefault();
            if (salarieFound != null)
            {
                salarieFound = updatedSalarie;
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            return salarieFound;
        }
        public void DeleteSalarie(int pID)
        {
            Salarie salarieFound = _CEDAlfaiaAppContextDatabase.Salaries.Where(s => s.ID == pID).FirstOrDefault();
            if (salarieFound != null)
            {
                _CEDAlfaiaAppContextDatabase.Salaries.Remove(salarieFound);
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
    }
}
