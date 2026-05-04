using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Parametrage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using CEDAlfaiaApp.DAL.Widgets;

namespace CEDAlfaiaApp.BLL
{
    public class WidgetService
    {
        private CEDAlfaiaAppContext _CEDAlfaiaAppContextDatabase;

        internal WidgetService(CEDAlfaiaAppContext pCEDAlfaiaAppContextDatabase)
        {
            _CEDAlfaiaAppContextDatabase = pCEDAlfaiaAppContextDatabase;
        }

        public IQueryable<Widget> GetAllWidgets()
        {
            return _CEDAlfaiaAppContextDatabase.Widgets;
        }
        public Widget GetWidget(int id)
        {
            return _CEDAlfaiaAppContextDatabase.Widgets.Where(c => c.ID == id).FirstOrDefault();
        }
        public IQueryable<WidgetUtilisateur> GetWidgetsUtilisateurByUtilisateur(int pUtilisateurID)
        {
            return _CEDAlfaiaAppContextDatabase.WidgetsUtilisateurs.Where(w => w.CompteUtilisateurID == pUtilisateurID);
        }
        public WidgetUtilisateur GetWidgetsUtilisateurByID(int pWidgetUtilisateurID)
        {
            return _CEDAlfaiaAppContextDatabase.WidgetsUtilisateurs.Where(w => w.ID == pWidgetUtilisateurID).FirstOrDefault();
        }
        public Widget AddUpdateWidget(int pID, string pLibelle, string pHtmlName, string pHtmlPanelUID, string pHtmlPartialWidget, string pHtmlWidth)
        {
            Widget widgetResult = new Widget();
            if (pID > 0)
                widgetResult = _CEDAlfaiaAppContextDatabase.Widgets.Where(c => c.ID == pID).FirstOrDefault();

            widgetResult.Libelle = pLibelle;
            widgetResult.HtmlName = pHtmlName;
            widgetResult.HtmlPanelUID = pHtmlPanelUID;
            widgetResult.HtmlPartialWidget = pHtmlPartialWidget;
            widgetResult.HtmlWidth = pHtmlWidth;

            if (pID == 0)
                widgetResult = _CEDAlfaiaAppContextDatabase.Widgets.Add(widgetResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return widgetResult;
        }
        public void DeleteWidget(int pID)
        {
            Widget widget = new Widget();
            if (pID > 0)
                widget = _CEDAlfaiaAppContextDatabase.Widgets.Where(c => c.ID == pID).FirstOrDefault();

            _CEDAlfaiaAppContextDatabase.Widgets.Remove(widget);
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public WidgetUtilisateur AddUpdateWidgetUtilisateur(int pID, int pCompteUtilisateurID, int pWidgetID, string pTitre, int? pHtmlIndex, 
            string pHtmlLeft, string pHtmlTop, string pHtmlOwnerZoneUID, DateTime? pDateDebut, DateTime? pDateFin, bool? pIsAnneeEnCours, bool? pIsMoisEnCours)
        {
            WidgetUtilisateur widgetUtilisateurResult = new WidgetUtilisateur();
            if (pID > 0)
                widgetUtilisateurResult = _CEDAlfaiaAppContextDatabase.WidgetsUtilisateurs.Where(c => c.ID == pID).FirstOrDefault();

            widgetUtilisateurResult.Titre = pTitre;
            widgetUtilisateurResult.CompteUtilisateurID = pCompteUtilisateurID;
            widgetUtilisateurResult.WidgetID = pWidgetID;
            widgetUtilisateurResult.HtmlIndex = pHtmlIndex;
            widgetUtilisateurResult.HtmlLeft = pHtmlLeft;
            widgetUtilisateurResult.HtmlTop = pHtmlTop;
            widgetUtilisateurResult.HtmlOwnerZoneUID = pHtmlOwnerZoneUID;
            widgetUtilisateurResult.DateDebut = pDateDebut;
            widgetUtilisateurResult.DateFin = pDateFin;
            widgetUtilisateurResult.IsAnneeEnCours = pIsAnneeEnCours;
            widgetUtilisateurResult.IsMoisEnCours = pIsMoisEnCours;

            if (pID == 0)
                widgetUtilisateurResult = _CEDAlfaiaAppContextDatabase.WidgetsUtilisateurs.Add(widgetUtilisateurResult);

            _CEDAlfaiaAppContextDatabase.SaveChanges();
            return widgetUtilisateurResult;
        }
        public void DeleteWidgetUtilisateur(int pID)
        {
            WidgetUtilisateur widgetUtilisateur = new WidgetUtilisateur();
            if (pID > 0)
                widgetUtilisateur = _CEDAlfaiaAppContextDatabase.WidgetsUtilisateurs.Where(c => c.ID == pID).FirstOrDefault();

            _CEDAlfaiaAppContextDatabase.WidgetsUtilisateurs.Remove(widgetUtilisateur);
            _CEDAlfaiaAppContextDatabase.SaveChanges();
        }
        public void SavePositionWidget(int pWidgetUtilisateurID, string zone, string top, string left)
        {
            var widgetUtilisateur = _CEDAlfaiaAppContextDatabase.WidgetsUtilisateurs.Where(w => w.ID == pWidgetUtilisateurID).FirstOrDefault();
            if (widgetUtilisateur != null)
            {
                widgetUtilisateur.HtmlTop = top;
                widgetUtilisateur.HtmlOwnerZoneUID = zone;
                widgetUtilisateur.HtmlLeft = left;
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }

        public void InitWidgets()
        {
            if (!_CEDAlfaiaAppContextDatabase.Widgets.Any(w => w.HtmlName == "ChiffreDAffaire"))
            {
                var widgets = _CEDAlfaiaAppContextDatabase.Widgets.ToList();
                _CEDAlfaiaAppContextDatabase.Widgets.Add(new Widget()
                {
                    Libelle = "Chiffre d'affaire",
                    HtmlName = "ChiffreDAffaire",
                    HtmlPanelUID = "ChiffreDAffaire",
                    HtmlPartialWidget = "_WidgetCA",
                    HtmlWidth = "200px"
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            if (!_CEDAlfaiaAppContextDatabase.Widgets.Any(w => w.HtmlName == "Marge"))
            {
                var widgets = _CEDAlfaiaAppContextDatabase.Widgets.ToList();
                _CEDAlfaiaAppContextDatabase.Widgets.Add(new Widget()
                {
                    Libelle = "Marge",
                    HtmlName = "Marge",
                    HtmlPanelUID = "Marge",
                    HtmlPartialWidget = "_WidgetMarge",
                    HtmlWidth = "200px"
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            if (!_CEDAlfaiaAppContextDatabase.Widgets.Any(w => w.HtmlName == "Objectif"))
            {
                var widgets = _CEDAlfaiaAppContextDatabase.Widgets.ToList();
                _CEDAlfaiaAppContextDatabase.Widgets.Add(new Widget()
                {
                    Libelle = "Objectif",
                    HtmlName = "Objectif",
                    HtmlPanelUID = "Objectif",
                    HtmlPartialWidget = "_WidgetObjectif",
                    HtmlWidth = "200px"
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            if (!_CEDAlfaiaAppContextDatabase.Widgets.Any(w => w.HtmlName == "Calendrier"))
            {
                var widgets = _CEDAlfaiaAppContextDatabase.Widgets.ToList();
                _CEDAlfaiaAppContextDatabase.Widgets.Add(new Widget()
                {
                    Libelle = "Calendrier",
                    HtmlName = "Calendrier",
                    HtmlPanelUID = "Calendrier",
                    HtmlPartialWidget = "_WidgetCalendrier",
                    HtmlWidth = "200px"
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            if (!_CEDAlfaiaAppContextDatabase.Widgets.Any(w => w.HtmlName == "ChiffreDAffaireParMois"))
            {
                var widgets = _CEDAlfaiaAppContextDatabase.Widgets.ToList();
                _CEDAlfaiaAppContextDatabase.Widgets.Add(new Widget()
                {
                    Libelle = "Chiffre d'affaire par mois",
                    HtmlName = "ChiffreDAffaireParMois",
                    HtmlPanelUID = "ChiffreDAffaireParMois",
                    HtmlPartialWidget = "_WidgetCAParMois",
                    HtmlWidth = "200px"
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
            if (!_CEDAlfaiaAppContextDatabase.Widgets.Any(w => w.HtmlName == "MargeParMois"))
            {
                var widgets = _CEDAlfaiaAppContextDatabase.Widgets.ToList();
                _CEDAlfaiaAppContextDatabase.Widgets.Add(new Widget()
                {
                    Libelle = "Marge par mois",
                    HtmlName = "MargeParMois",
                    HtmlPanelUID = "MargeParMois",
                    HtmlPartialWidget = "_WidgetMargeParMois",
                    HtmlWidth = "200px"
                });
                _CEDAlfaiaAppContextDatabase.SaveChanges();
            }
        }
    }
}
