using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Shared
{
	public class Adresse
	{
		[StringLength(50)]
		[Column(TypeName = "VARCHAR")]
		public string Ligne1 { get; set; }
		[StringLength(50)]
		[Column(TypeName = "VARCHAR")]
		public string Ligne2 { get; set; }
		[StringLength(50)]
		[Column(TypeName = "VARCHAR")]
		public string Ligne3 { get; set; }
		[StringLength(50)]
		[Column(TypeName = "VARCHAR")]
		public string CodePostal { get; set; }
		[StringLength(50)]
		[Column(TypeName = "VARCHAR")]
		public string Ville { get; set; }
		[StringLength(50)]
		[Column(TypeName = "VARCHAR")]
		public string Pays { get; set; }
	}
}
