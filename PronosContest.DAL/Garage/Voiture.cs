using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEDAlfaiaApp.DAL.Garage
{
	public class Voiture
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }
        
		[StringLength(20)]
		[Column(TypeName = "VARCHAR")]
		public string Immatriculation { get; set; }
        
        [ForeignKey("Modele")]
        public int? ModeleID { get; set; }

        [ForeignKey("Client")]
        public int? ClientID { get; set; }

        public bool IsPrincipale { get; set; }
        
        public DateTime? DateSuppression { get; set; }

        #endregion

        public Voiture()
		{

		}

        #region Propriétés de navigation
        public virtual Modele Modele { get; set; }
        public virtual Client Client { get; set; }
        #endregion
    }
}
