using CEDAlfaiaApp.DAL.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEDAlfaiaApp.DAL.Widgets
{
    public class Widget
	{
		#region Propriétés primitives
		[Key]
		public int ID { get; set; }
        
        public string Libelle { get; set; }
        public string HtmlName { get; set; }
        public string HtmlPanelUID { get; set; }
        public string HtmlPartialWidget { get; set; }
        public string HtmlWidth { get; set; }
        #endregion

        public Widget()
		{

		}

        #region Propriétés de navigation
        #endregion
    }
}
