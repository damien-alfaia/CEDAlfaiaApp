using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Widgets
{
    public class WidgetUtilisateur
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }

        [ForeignKey("Widget")]
        public int? WidgetID { get; set; }

        [ForeignKey("CompteUtilisateur")]
        public int? CompteUtilisateurID { get; set; }

        public string Titre { get; set; }

        public string HtmlOwnerZoneUID { get; set; }
        public int? HtmlIndex { get; set; }
        public string HtmlTop { get; set; }
        public string HtmlLeft { get; set; }

        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public bool? IsMoisEnCours { get; set; }
        public bool? IsAnneeEnCours { get; set; }
        #endregion

        public WidgetUtilisateur()
		{

		}

        #region Propriétés de navigation
        public virtual CompteUtilisateur CompteUtilisateur { get; set; }
        public virtual Widget Widget { get; set; }
        #endregion
    }
}
