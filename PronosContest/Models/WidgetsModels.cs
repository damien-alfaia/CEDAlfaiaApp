using CEDAlfaiaApp.DAL.Widgets;
using System;
using System.ComponentModel.DataAnnotations;

namespace CEDAlfaiaAppV2.Models
{

    public class WidgetUtilisateurModel
    {
        public int ID { get; set; }
        [Display(Name = "Titre")]
        public string Titre { get; set; }
        [Display(Name = "Date de début")]
        public DateTime? DateDebut { get; set; }
        [Display(Name = "Date de fin")]
        public DateTime? DateFin { get; set; }
        [Display(Name = "Année en cours")]
        public bool? IsAnneeEnCours { get; set; }
        [Display(Name = "Mois en cours")]
        public bool? IsMoisEnCours { get; set; }

        public int? CompteUtilisateurID { get; set; }
        [Display(Name = "Widget")]
        public int? WidgetID { get; set; }

        public string HtmlTop { get; set; }
        public string HtmlLeft { get; set; }
        public string HtmlOwnerZoneUID { get; set; }
        public int? HtmlIndex { get; set; }

        public WidgetUtilisateurModel()
        {
            this.ID = 0;
        }
        public WidgetUtilisateurModel(WidgetUtilisateur pWidgetUtilisateur)
        {
            this.ID = 0;
            if (pWidgetUtilisateur != null)
            {
                this.ID = pWidgetUtilisateur.ID;
                this.Titre = pWidgetUtilisateur.Titre;
                this.DateDebut = pWidgetUtilisateur.DateDebut;
                this.DateFin = pWidgetUtilisateur.DateFin;
                this.IsAnneeEnCours = pWidgetUtilisateur.IsAnneeEnCours;
                this.IsMoisEnCours = pWidgetUtilisateur.IsMoisEnCours;
                this.CompteUtilisateurID = pWidgetUtilisateur.CompteUtilisateurID;
                this.WidgetID = pWidgetUtilisateur.WidgetID;
                this.HtmlTop = pWidgetUtilisateur.HtmlTop;
                this.HtmlLeft = pWidgetUtilisateur.HtmlLeft;
                this.HtmlOwnerZoneUID = pWidgetUtilisateur.HtmlOwnerZoneUID;
                this.HtmlIndex = pWidgetUtilisateur.HtmlIndex;
            }
        }
    }
}
