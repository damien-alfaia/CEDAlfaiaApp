using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.DAL.Shared;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Models
{
	public class LogInModel
	{
		public string Email { get; set; }

		public string Password { get; set; }

		[HiddenInput]
		public string ReturnUrl { get; set; }
	}

	public class InscriptionModel
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password2 { get; set; }

		[Required]
		[DataType(DataType.Text)]
		public string Prenom { get; set; }

		[Required]
		[DataType(DataType.Text)]
		public string Nom { get; set; }

		public Adresse Adresse { get; set; }

        public CompteUtilisateurRole Role { get; set; }
	}
}
