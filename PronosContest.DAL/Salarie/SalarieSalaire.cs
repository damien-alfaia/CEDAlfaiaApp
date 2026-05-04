using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Salaries
{
    public class SalarieSalaire
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }
        
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public DateTime DatePaiement { get; set; }
        public float SalaireNet { get; set; }

        [ForeignKey("SalarieContrat")]
        public int? SalarieContratID { get; set; }
        #endregion

        public SalarieSalaire()
		{

		}

        #region Propriétés de navigation
        public virtual SalarieContrat SalarieContrat { get; set; }
        #endregion
    }
}
