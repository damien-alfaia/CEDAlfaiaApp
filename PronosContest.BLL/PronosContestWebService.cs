using CEDAlfaiaApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CEDAlfaiaApp.BLL
{
    public class CEDAlfaiaAppWebService : IDisposable
    {
		private const string CEDAlfaiaApp_WEB_SERVICE_NAME = "CEDAlfaiaAppWebService";

		private CEDAlfaiaAppContext _CEDAlfaiaAppContextDatabase;
		public static CEDAlfaiaAppWebServiceContext CEDAlfaiaAppWebServiceContext { get; set; }

		static CEDAlfaiaAppWebService()
		{
			CEDAlfaiaAppWebServiceContext = new CEDAlfaiaAppWebServiceContext();
		}
		private CEDAlfaiaAppWebService()
		{
			_CEDAlfaiaAppContextDatabase = new CEDAlfaiaAppContext();
		}
		public static CEDAlfaiaAppWebService GetService()
		{
			var CEDAlfaiaAppWebService = CEDAlfaiaAppWebServiceContext.HttpContext.Items[CEDAlfaiaApp_WEB_SERVICE_NAME] as CEDAlfaiaAppWebService;
			if (CEDAlfaiaAppWebService == null)
			{
				CEDAlfaiaAppWebService = new CEDAlfaiaAppWebService();
				CEDAlfaiaAppWebServiceContext.HttpContext.Items[CEDAlfaiaApp_WEB_SERVICE_NAME] = CEDAlfaiaAppWebService;
			}
			return CEDAlfaiaAppWebService;
		}
		
		void IDisposable.Dispose()
		{
			_CEDAlfaiaAppContextDatabase.Dispose();
			CEDAlfaiaAppWebServiceContext.HttpContext.Items.Remove(CEDAlfaiaApp_WEB_SERVICE_NAME);
		}
		public AuthentificationService AuthenticationService
		{
			get
			{
				return new AuthentificationService(_CEDAlfaiaAppContextDatabase);
			}
		}
		public GarageService GarageService
        {
			get
			{
				return new GarageService(_CEDAlfaiaAppContextDatabase);
			}
		}

        public ParametrageService ParametrageService
        {
            get
            {
                return new ParametrageService(_CEDAlfaiaAppContextDatabase);
            }
        }
        public WidgetService WidgetService
        {
            get
            {
                return new WidgetService(_CEDAlfaiaAppContextDatabase);
            }
        }
        public StartService StartService
		{
			get
			{
				return new StartService(_CEDAlfaiaAppContextDatabase);
			}
        }
        public SalariesService SalariesService
        {
            get
            {
                return new SalariesService(_CEDAlfaiaAppContextDatabase);
            }
        }
    }
}
