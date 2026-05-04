using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Garage
{
    public class Modele
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }

        [ForeignKey("Marque")]
        public int? MarqueID { get; set; }

        [Required]
        [StringLength(500)]
        [Column(TypeName = "VARCHAR")]
        public string Libelle { get; set; }
        #endregion

        public Modele()
		{

		}

        #region Propriétés de navigation
        public virtual Marque Marque { get; set; }
		#endregion
    }
}
