using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Salaries;

namespace CEDAlfaiaAppV2.Controllers
{
    [Route("api/SalariesApi/{action}", Name = "SalariesApi")]
    public class SalariesApiController : ApiController
    {
        private CEDAlfaiaAppContext db = new CEDAlfaiaAppContext();

        // GET: api/SalariesApi
        public IQueryable<Salarie> GetSalaries()
        {
            return db.Salaries;
        }

        // GET: api/SalariesApi/5
        [ResponseType(typeof(Salarie))]
        public IHttpActionResult GetSalarie(int id)
        {
            Salarie salarie = db.Salaries.Find(id);
            if (salarie == null)
            {
                return NotFound();
            }

            return Ok(salarie);
        }

        // PUT: api/SalariesApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutSalarie(int id, Salarie salarie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salarie.ID)
            {
                return BadRequest();
            }

            db.Entry(salarie).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalarieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/SalariesApi
        [ResponseType(typeof(Salarie))]
        public IHttpActionResult PostSalarie(Salarie salarie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (salarie.Adresse == null)
                salarie.Adresse = new CEDAlfaiaApp.DAL.Shared.Adresse();

            db.Salaries.Add(salarie);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = salarie.ID }, salarie);
        }

        // DELETE: api/SalariesApi/5
        [ResponseType(typeof(Salarie))]
        public IHttpActionResult DeleteSalarie(int id)
        {
            Salarie salarie = db.Salaries.Find(id);
            if (salarie == null)
            {
                return NotFound();
            }

            db.Salaries.Remove(salarie);
            db.SaveChanges();

            return Ok(salarie);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SalarieExists(int id)
        {
            return db.Salaries.Count(e => e.ID == id) > 0;
        }
    }
}