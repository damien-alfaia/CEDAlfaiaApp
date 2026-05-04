using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace CEDAlfaiaAppV2 {

    public class MvcApplication : HttpApplication {
        protected void Application_Start() {
            DevExpress.XtraReports.Web.WebDocumentViewer.Native.WebDocumentViewerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
            DevExpress.XtraReports.Web.QueryBuilder.Native.QueryBuilderBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
            DevExpress.XtraReports.Web.ReportDesigner.Native.ReportDesignerBootstrapper.SessionState = System.Web.SessionState.SessionStateBehavior.Disabled;
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);

            // Uncomment to use pre-17.2 routing for .Mvc() and .WebApi() data sources
            DevExtreme.AspNet.Mvc.Compatibility.DataSource.UseLegacyRouting = true;
            // Uncomment to use pre-17.2 behavior for the "required" validation check
            DevExtreme.AspNet.Mvc.Compatibility.Validation.IgnoreRequiredForBoolean = false;
            //DevExpress.Web.Mvc.MVCxReportDesigner.StaticInitialize();
        }
    }
}
