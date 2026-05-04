using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public class Marque
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }

		[Required]
		[StringLength(5)]
		[Column(TypeName = "VARCHAR")]
		public string Code { get; set; }

        [Required]
        [StringLength(500)]
        [Column(TypeName = "VARCHAR")]
        public string Libelle { get; set; }
        #endregion

        public Marque()
		{
            this.Modeles = new List<Modele>();
		}
        
        #region Propriétés de navigation
        public virtual ICollection<Modele> Modeles { get; set; }
		#endregion
    }
}
