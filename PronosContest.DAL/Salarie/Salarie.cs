using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Salaries
{
    [Serializable()]
    public class Salarie
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }
        
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public DateTime DateNaissance { get; set; }
        public Adresse Adresse { get; set; }
        public string Telephone { get; set; }
        public string Portable { get; set; }
        public string Email { get; set; }

        #endregion

        public Salarie()
		{
            this.Adresse = new Adresse();
		}

        #region Propriétés de navigation
        //public virtual ICollection<SalarieContrat> Contrats { get; set; }
        #endregion
    }
}
