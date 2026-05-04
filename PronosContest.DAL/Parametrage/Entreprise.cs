using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CEDAlfaiaApp.DAL.Authentification;

namespace CEDAlfaiaApp.DAL.Parametrage
{
    public class Entreprise
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }

		[Required]
		[StringLength(256)]
		[Column(TypeName = "VARCHAR")]
		public string Nom { get; set; }
        
        public Adresse Adresse { get; set; }
        
        public string Siren { get; set; }
        public string Siret { get; set; }
        
        [StringLength(5000)]
        [Column(TypeName = "VARCHAR")]
        public string Commentaire { get; set; }

        public DateTime DateCreation { get; set; }

        public bool IsActif { get; set; }

        [ForeignKey("Parametrage")]
        public int? ParametrageID { get; set; }
        #endregion

        public Entreprise()
		{

		}
        
        #region Propriétés de navigation
        public virtual ICollection<CompteUtilisateur> Utilisateurs { get; set; }
        public virtual Parametrage Parametrage { get; set; }
        #endregion
    }
}
