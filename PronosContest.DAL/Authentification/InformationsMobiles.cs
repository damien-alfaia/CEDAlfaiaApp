using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEDAlfaiaApp.DAL.Authentification
{
	public enum TypeDePlateformeMobile
	{
		Android = 0,
		IOS = 1,
		WindowsPhone = 2
	}
	public class InformationsMobile
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }

		public string Jeton { get; set; }
		public TypeDePlateformeMobile PlateformeMobile { get; set; }

		[ForeignKey("CompteUtilisateur")]
		public int IDCompteUtilisateur { get; set; }
		#endregion

		#region Propriétés de navigation
		public virtual CompteUtilisateur CompteUtilisateur { get; set; }
		#endregion
	}
}
