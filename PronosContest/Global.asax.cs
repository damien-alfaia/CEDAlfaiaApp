using CEDAlfaiaApp.BLL;
using DevExpress.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CEDAlfaiaApp
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);

            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
            DevExpress.XtraReports.Web.QueryBuilder.Native.QueryBuilderBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
            DevExpress.XtraReports.Web.ReportDesigner.Native.ReportDesignerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("fr-fr");

            CEDAlfaiaAppWebService.GetService().StartService.Init();
            DevExpress.Web.Mvc.MVCxReportDesigner.StaticInitialize();
        }
        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            DevExpressHelper.Theme = "Metropolis";
        }

    }
}
