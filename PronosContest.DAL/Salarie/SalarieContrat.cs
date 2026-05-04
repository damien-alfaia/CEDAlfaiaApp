using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Salaries
{
    public class SalarieContrat
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }
        
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public string TypeContrat { get; set; }

        [ForeignKey("Salarie")]
        public int? SalarieID { get; set; }
        #endregion

        public SalarieContrat()
		{

		}

        #region Propriétés de navigation
        public virtual Salarie Salarie { get; set; }
        //public virtual ICollection<SalarieSalaire> Salaires { get; set; }
        //public virtual ICollection<SalarieIndisponibilite> Indisponibilites { get; set; }
        #endregion
    }
}
