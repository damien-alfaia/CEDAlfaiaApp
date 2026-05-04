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
using System.Web.Mvc;
using CEDAlfaiaApp.DAL;
using CEDAlfaiaApp.DAL.Salaries;
using CEDAlfaiaAppV2.Code;
using CEDAlfaiaAppV2.Models;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Newtonsoft.Json;

namespace CEDAlfaiaAppV2.Controllers
{
    public class SalariesController : CEDAlfaiaAppControllerBase
    {
        private CEDAlfaiaAppContext db = new CEDAlfaiaAppContext();
        const string ValidationErrorMessage = "L'enregistrement n'a pas pu être effectué !";

        public List<KeyValue> GetTypesIndisponibilites()
        {
            List<KeyValue> typeIndisponibilites = new List<KeyValue>();
            foreach (TypeIndisponibilite typeIndisponibilite in Enum.GetValues(typeof(TypeIndisponibilite)))
            {
                typeIndisponibilites.Add(new KeyValue((int)typeIndisponibilite, typeIndisponibilite.ToString()));
            }
            return typeIndisponibilites;
        }

        #region Salaries
        public ActionResult GetSalaries(DataSourceLoadOptions loadOptions)
        {
            var result = DataSourceLoader.Load(db.Salaries.ToList(), loadOptions);
            var resultJson = JsonConvert.SerializeObject(result);
            return Content(resultJson, "application/json");
        }
        public ActionResult InsertSalarie(string values)
        {
            var newSalarie = new Salarie();
            JsonConvert.PopulateObject(values, newSalarie);           // Populating the item with the values
            if (!TryValidateModel(newSalarie))                        // Validating the item
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.Salaries.Add(newSalarie);                            // Adding the item to the database
            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        public ActionResult UpdateSalarie(int key, string values)
        {
            var salarie = db.Salaries.First(s => s.ID == key); // Finding the item to be updated by key
            JsonConvert.PopulateObject(values, salarie);              // Populating the found item with the changed values
            if (!TryValidateModel(salarie))                           // Validating the updated item
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }
        public void DeleteSalarie(int key)
        {
            var salarie = db.Salaries.First(s => s.ID == key); // Finding the item to be removed by key
            db.Salaries.Remove(salarie);                            // Removing the found item
            db.SaveChanges();
        }
        #endregion

        #region Contrats
        public ActionResult GetContrats(int idSalarie, DataSourceLoadOptions loadOptions)
        {
            var result = DataSourceLoader.Load(db.SalarieContrats.Where(sc => sc.SalarieID == idSalarie).ToList(), loadOptions);
            var resultJson = JsonConvert.SerializeObject(result);
            return Content(resultJson, "application/json");
        }
        public ActionResult InsertContrat(string values)
        {
            var newContrat = new SalarieContrat();
            JsonConvert.PopulateObject(values, newContrat);
            if (!TryValidateModel(newContrat))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.SalarieContrats.Add(newContrat);
            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        public ActionResult UpdateContrat(int key, string values)
        {
            var contrat = db.SalarieContrats.First(s => s.ID == key);
            JsonConvert.PopulateObject(values, contrat);
            if (!TryValidateModel(contrat))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }
        public void DeleteContrat(int key)
        {
            var contrat = db.SalarieContrats.First(s => s.ID == key);
            db.SalarieContrats.Remove(contrat);
            db.SaveChanges();
        }
        #endregion

        #region Salaires
        public ActionResult GetSalaires(int idSalarieContrat, DataSourceLoadOptions loadOptions)
        {
            var result = DataSourceLoader.Load(db.SalarieSalaires.Where(sc => sc.SalarieContratID == idSalarieContrat).ToList(), loadOptions);
            var resultJson = JsonConvert.SerializeObject(result);
            return Content(resultJson, "application/json");
        }
        public ActionResult InsertSalaire(string values)
        {
            var newSalaire = new SalarieSalaire();
            JsonConvert.PopulateObject(values, newSalaire);
            if (!TryValidateModel(newSalaire))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.SalarieSalaires.Add(newSalaire);
            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        public ActionResult UpdateSalaire(int key, string values)
        {
            var salaire = db.SalarieSalaires.First(s => s.ID == key);
            JsonConvert.PopulateObject(values, salaire);
            if (!TryValidateModel(salaire))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }
        public void DeleteSalaire(int key)
        {
            var salaire = db.SalarieSalaires.First(s => s.ID == key);
            db.SalarieSalaires.Remove(salaire);
            db.SaveChanges();
        }
        #endregion

        #region Indisponibilités
        public ActionResult GetIndisponibilites(int idSalarieContrat, DataSourceLoadOptions loadOptions)
        {
            var result = DataSourceLoader.Load(db.SalarieIndisponibilites.Where(sc => sc.SalarieContratID == idSalarieContrat).ToList(), loadOptions);
            var resultJson = JsonConvert.SerializeObject(result);
            return Content(resultJson, "application/json");
        }
        public ActionResult InsertIndisponibilite(string values)
        {
            var newIndisponibilite = new SalarieIndisponibilite();
            JsonConvert.PopulateObject(values, newIndisponibilite);
            if (!TryValidateModel(newIndisponibilite))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.SalarieIndisponibilites.Add(newIndisponibilite);
            db.SaveChanges();

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        public ActionResult UpdateIndisponibilite(int key, string values)
        {
            var indisponibilite = db.SalarieIndisponibilites.First(s => s.ID == key);
            JsonConvert.PopulateObject(values, indisponibilite);
            if (!TryValidateModel(indisponibilite))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ValidationErrorMessage);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }
        public void DeleteIndisponibilite(int key)
        {
            var indisponibilite = db.SalarieIndisponibilites.First(s => s.ID == key);
            db.SalarieIndisponibilites.Remove(indisponibilite);
            db.SaveChanges();
        }
        #endregion
    }
}