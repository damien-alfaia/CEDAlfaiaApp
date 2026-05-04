using CEDAlfaiaApp.BLL;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaAppV2.Code;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CEDAlfaiaAppV2.Controllers
{
    public class HomeController : CEDAlfaiaAppControllerBase
    {
        public ActionResult Index()
        {
            return RedirectToAction("Widgets", "Widgets");
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult IntegrationDonnees()
        {
            return View();
        }
        public ActionResult ChargerDonnees()
        {
            var pathTest = this.HttpContext.Request.PhysicalApplicationPath + "App_Data\\";
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(pathTest));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            SqlCeConnection dbCon = new SqlCeConnection("Data Source=|DataDirectory|\\garage.sdf;Encrypt Database=False;Password=benalf;");
            dbCon.Open();

            SqlCeCommand command;
            SqlCeDataReader reader;

            #region Clients
            List<Client> clients = new List<Client>();
            command = new SqlCeCommand("select * from Client", dbCon);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string nom = reader["Nom"].ToString();
                string telephone = reader["Telephone"].ToString();
                string informations = reader["Informations"].ToString();

                Client client = new Client()
                {
                    ID = id,
                    Nom = nom,
                    Telephone = telephone,
                    InformationsComplementaires = informations
                };

                clients.Add(client);
            }
            #endregion

            #region Marques
            List<Marque> marques = new List<Marque>();
            command = new SqlCeCommand("select * from Marque", dbCon);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string value = reader["Value"].ToString();
                string text = reader["Text"].ToString();

                Marque marque = new Marque()
                {
                    ID = id,
                    Code = value,
                    Libelle = text
                };

                marques.Add(marque);
            }
            #endregion

            #region Modèles
            List<Modele> modeles = new List<Modele>();
            command = new SqlCeCommand("select * from Modele", dbCon);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                string marque = reader["Marque"].ToString();
                string libelle = reader["Valeur"].ToString();

                Marque marqueFound = marques.Where(m => m.Code == marque).FirstOrDefault();
                Modele modele = new Modele()
                {
                    ID = id,
                    Marque = marqueFound,
                    MarqueID = marqueFound != null ? marqueFound.ID : 0,
                    Libelle = libelle
                };

                modeles.Add(modele);
            }
            #endregion

            #region Voitures
            List<Voiture> voitures = new List<Voiture>();
            command = new SqlCeCommand("select * from Voiture", dbCon);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int idClient = reader.GetInt32(1);
                string immatriculation = reader["Immatriculation"].ToString();
                int modele = reader.GetInt32(4);
                string isPrincipale = reader["isPrincipal"].ToString();

                Voiture voiture = new Voiture()
                {
                    ID = id,
                    Immatriculation = immatriculation,
                    ModeleID = modele,
                    Modele = modeles.Where(m => m.ID == modele).FirstOrDefault(),
                    ClientID = idClient,
                    Client = clients.Where(c => c.ID == idClient).FirstOrDefault(),
                    IsPrincipale = isPrincipale == "1" ? true : false
                };

                voitures.Add(voiture);
            }
            #endregion

            #region Devis et factures
            List<PieceVente> devis = new List<PieceVente>();
            List<PieceVente> factures = new List<PieceVente>();
            command = new SqlCeCommand("select * from Devis order by CONVERT(DATETIME, Date, 103)", dbCon);
            reader = command.ExecuteReader();
            int no = 0;
            int annee = 0;
            /*
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int idClient = reader.GetInt32(1);
                int idVoiture = reader.GetInt32(2);
                string dateStr = reader.GetString(3);
                DateTime date;
                DateTime.TryParse(dateStr, out date);
                double reste = reader.GetDouble(4);
                int kilometrage = reader.GetInt32(5);
                int numFacture = 0;
                if (!reader.IsDBNull(6))
                    numFacture = reader.GetInt32(6);
                int anneeFacture = 0;
                if (!reader.IsDBNull(7))
                    anneeFacture = reader.GetInt32(7);

                if (annee != date.Year)
                {
                    annee = date.Year;
                    no = 1;
                }

                numFacture = no;
                anneeFacture = annee;

                no++;

                Devis newDevis = new Devis()
                {
                    ID = id,
                    ClientID = idClient,
                    Client = clients.Where(c => c.ID == idClient).FirstOrDefault(),
                    Date = date,
                    Voiture = voitures.Where(v => v.ID == idVoiture).FirstOrDefault(),
                    VoitureID = idVoiture,
                    NumDevis = numFacture
                };

                Facture newFacture = new Facture()
                {
                    ID = id,
                    DevisID = newDevis.ID,
                    Devis = newDevis,
                    Date = date,
                    Kilometrage = kilometrage,
                    Reste = float.Parse(reste.ToString()),
                    NumFacture = numFacture
                };

                devis.Add(newDevis);
                factures.Add(newFacture);
            }
            */
            #endregion

            #region Lignes devis
            /*List<DevisLigne> lignes = new List<DevisLigne>();
            command = new SqlCeCommand("select * from Devis_Reparations", dbCon);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = reader.GetInt32(0);
                int idDevis = reader.GetInt32(1);
                string nom = reader.GetString(2);
                double remise = reader.GetDouble(3);
                double prixGarageHT = reader.GetDouble(4);
                double prixGarageTTC = reader.GetDouble(5);
                double prixClientHT = reader.GetDouble(6);
                double prixClientTTC = reader.GetDouble(7);
                int quantite = 1;
                if (!reader.IsDBNull(8))
                    quantite = reader.GetInt32(8);

                DevisLigne ligne = new DevisLigne()
                {
                    ID = id,
                    DevisID = idDevis,
                    Devis = devis.Where(d => d.ID == idDevis).FirstOrDefault(),
                    Libelle = nom,
                    Remise = float.Parse(remise.ToString()), 
                    PrixGarageHT = float.Parse(prixGarageHT.ToString()),
                    PrixGarageTTC = float.Parse(prixGarageTTC.ToString()),
                    PrixClientHT = float.Parse(prixClientHT.ToString()),
                    PrixClientTTC = float.Parse(prixClientTTC.ToString()),
                    Quantite = quantite
                };

                lignes.Add(ligne);
            }*/
            #endregion

            dbCon.Close();

            CEDAlfaiaAppWebService.GetService().GarageService.Init(clients, marques, modeles, voitures);

            return RedirectToAction("Index");
        }

        public ActionResult ChargerVehicules()
        {
            string path = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

            using (StreamReader r = new StreamReader(path + "\\bin\\all-vehicles-model.json"))
            {
                using (JsonReader reader = new JsonTextReader(r))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    object lstObjects = serializer.Deserialize<object>(reader);
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult SeDeconnecter()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut();
            return RedirectToAction("Index");
        }
    }
}