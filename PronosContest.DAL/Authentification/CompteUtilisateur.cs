using CEDAlfaiaApp.Core;
using CEDAlfaiaApp.DAL.Parametrage;
using CEDAlfaiaApp.DAL.Shared;
using CEDAlfaiaApp.DAL.Widgets;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CEDAlfaiaApp.DAL.Authentification
{
	public enum CompteUtilisateurRole
	{
        SuperAdmin = 0,
		Administrateur = 1,
		Secretaire = 2, 
        Technicien = 3
	}
	public class CompteUtilisateur
	{
		[Key]
		public int ID { get; set; }

		[Required]
		[StringLength(256)]
		[Column(TypeName = "VARCHAR")]
		public string Email { get; set; }

		[StringLength(256)]
		[Column(TypeName = "VARCHAR")]
		public string Prenom { get; set; }

		public string Nom { get; set; }
		
		public CompteUtilisateurRole Role { get; set; }

		[Required]
		public byte[] Password { get; set; }

		public Adresse Adresse { get; set; }

        [ForeignKey("Entreprise")]
        public int? EntrepriseID { get; set; }

        public CompteUtilisateur()
        {
            this.Role = CompteUtilisateurRole.Administrateur;
            this.Adresse = new Adresse();
        }
        public CompteUtilisateur(string pEmail,string pPassword,string pNom, string pPrenom, Adresse pAdresse, CompteUtilisateurRole pRole)
        {
            this.Email = pEmail;
            this.Password = pPassword.ToPasswordHash();
            this.Nom = pNom;
            this.Prenom = pPrenom;
            this.Role = pRole;
            this.Adresse = new Adresse();
        }
        
        public virtual ICollection<WidgetUtilisateur> Widgets { get; set; }
        public virtual Entreprise Entreprise { get; set; }
    }
}
