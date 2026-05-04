using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Salaries
{
    public enum TypeIndisponibilite
    {
        Absence = 0,
        Conges = 1,
        Ecole = 2,
        Autre = 3
    }
    public class SalarieIndisponibilite
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }
        
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public TypeIndisponibilite TypeIndisponibilite { get; set; }
        public string Motif { get; set; }

        [ForeignKey("SalarieContrat")]
        public int? SalarieContratID { get; set; }
        #endregion

        public SalarieIndisponibilite()
		{

		}

        #region Propriétés de navigation
        public virtual SalarieContrat SalarieContrat { get; set; }
        #endregion
    }
}
