using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CEDAlfaiaApp.BLL
{
    public class CEDAlfaiaAppWebServiceContext
    {
		HttpContextBase _context = null;
		public CEDAlfaiaAppWebServiceContext()
		{
			return;
		}
		public CEDAlfaiaAppWebServiceContext(HttpContextBase pContexte)
		{
			_context = pContexte;
			return;
		}
		public HttpContextBase HttpContext
		{
			get
			{
				if (_context != null) return _context;
				return new HttpContextWrapper(System.Web.HttpContext.Current);
			}
		}
	}
}
