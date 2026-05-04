using CEDAlfaiaApp.DAL.Parametrage;
using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEDAlfaiaApp.DAL.Garage
{
	public class Client
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }

        [StringLength(256)]
        [Column(TypeName = "VARCHAR")]
        public string Code { get; set; }

        [Required]
		[StringLength(256)]
		[Column(TypeName = "VARCHAR")]
		public string Nom { get; set; }
        
        [StringLength(256)]
        [Column(TypeName = "VARCHAR")]
        public string Prenom { get; set; }

        public Adresse Adresse { get; set; }

        [StringLength(5000)]
        [Column(TypeName = "VARCHAR")]
        public string InformationsComplementaires { get; set; }
        
        [StringLength(100)]
        [Column(TypeName = "VARCHAR")]
        public string Telephone { get; set; }

        [StringLength(256)]
        [Column(TypeName = "VARCHAR")]
        public string Email { get; set; }

        public float? Remise { get; set; }

        public DateTime? DateSuppression { get; set; }

        public bool IsProspect { get; set; }

        [ForeignKey("Entreprise")]
        public int? EntrepriseID { get; set; }
        #endregion

        public Client()
		{
			this.Voitures = new List<Voiture>();
            this.Adresse = new Adresse();
            this.IsProspect = true;
        }
                
        #region Propriétés de navigation
        public virtual ICollection<Voiture> Voitures { get; set; }
        public virtual Entreprise Entreprise { get; set; }
        public virtual ICollection<PieceVente> PiecesVente { get; set; }
        #endregion
    }

    public class BalanceAgee
    {
        public string NomClient { get; set; }
        public float? SeptJours { get; set; }
        public float? QuinzeJours { get; set; }
        public float? UnMois { get; set; }
        public float? DeuxMois { get; set; }
        public float? TroisMois { get; set; }
        public float? Plus { get; set; }
        public float? Total { get; set; }
    }
}
