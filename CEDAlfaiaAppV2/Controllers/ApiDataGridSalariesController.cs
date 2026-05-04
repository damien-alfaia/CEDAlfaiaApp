using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using CEDAlfaiaApp.DAL.Salaries;
using CEDAlfaiaApp.BLL;
namespace CEDAlfaiaAppV2.Controllers
{
    [Route("ApiDataGridSalaries/{action}", Name = "ApiDataGridSalaries")]
    public class ApiDataGridSalariesController : ApiController
    {        
        [HttpGet]
        public HttpResponseMessage Get(DataSourceLoadOptions loadOptions)
        {
            return Request.CreateResponse(DataSourceLoader.Load(CEDAlfaiaAppWebService.GetService().SalariesService.Salaries(), loadOptions));
        }

        [HttpPost]
        public HttpResponseMessage Post(FormDataCollection form)
        {
            var values = form.Get("values");

            var newSalarie = new Salarie();
            JsonConvert.PopulateObject(values, newSalarie);

            Validate(newSalarie);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "ERROR");

            CEDAlfaiaAppWebService.GetService().SalariesService.AddSalarie(newSalarie);

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [HttpPut]
        public HttpResponseMessage Put(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            var values = form.Get("values");
            var salarie = CEDAlfaiaAppWebService.GetService().SalariesService.GetSalarie(key);

            JsonConvert.PopulateObject(values, salarie);

            Validate(salarie);
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "");

            CEDAlfaiaAppWebService.GetService().SalariesService.UpdateSalarie(salarie);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpDelete]
        public void Delete(FormDataCollection form)
        {
            var key = Convert.ToInt32(form.Get("key"));
            CEDAlfaiaAppWebService.GetService().SalariesService.DeleteSalarie(key);
        }
    }
}
