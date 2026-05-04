using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public class Fournisseur
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }

		[Required]
		[StringLength(256)]
		[Column(TypeName = "VARCHAR")]
		public string Nom { get; set; }
        
        public Adresse Adresse { get; set; }
        
        [Required]
        [StringLength(5000)]
        [Column(TypeName = "VARCHAR")]
        public string Commentaire { get; set; }

        public DateTime? DateSuppression { get; set; }
        #endregion

        public Fournisseur()
		{

		}
        
        #region Propriétés de navigation
        public virtual ICollection<PieceVenteLigne> PieceVenteLignes { get; set; }
		#endregion
    }
}
