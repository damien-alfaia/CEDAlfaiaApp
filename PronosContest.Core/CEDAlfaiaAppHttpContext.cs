using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CEDAlfaiaApp.Core
{
    public static class CEDAlfaiaAppHttpContext
    {
        public static int? UserID
        {
            get
            {
                return Helper.GetIntFromString(HttpContext.Current.Session["UserID"] as string);
            }
			set
			{
				HttpContext.Current.Session["UserID"] = value;
            }
        }
        public static float? TauxTVA
        {
            get
            {
                return Helper.GetIntFromString(HttpContext.Current.Session["TauxTVA"] as string);
            }
            set
            {
                HttpContext.Current.Session["TauxTVA"] = value;
            }
        }
    }
}
