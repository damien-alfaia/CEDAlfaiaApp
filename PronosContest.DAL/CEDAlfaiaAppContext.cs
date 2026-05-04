using CEDAlfaiaApp.DAL.Authentification;
using CEDAlfaiaApp.DAL.Garage;
using CEDAlfaiaApp.DAL.Parametrage;
using CEDAlfaiaApp.DAL.Widgets;
using CEDAlfaiaApp.DAL.Salaries;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEDAlfaiaApp.DAL
{
    public class CEDAlfaiaAppContext : DbContext
    {
		public CEDAlfaiaAppContext():base("name=CEDAlfaiaAppConnectionString")
        {
			//Database.SetInitializer(new MigrateDatabaseToLatestVersion<CEDAlfaiaAppContext, ENTRE_AMIS.DAL.Migrations.Configuration>());
			return;
		}
		public DbSet<CompteUtilisateur> CompteUtilisateurs { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<PieceVente> PiecesVente { get; set; }
        public DbSet<PieceVenteLigne> PieceVenteLignes { get; set; }
        public DbSet<PieceVenteService> PieceVenteServices { get; set; }
        public DbSet<PieceVentePaiement> PieceVentePaiements { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<Marque> Marques { get; set; }
        public DbSet<Modele> Modeles { get; set; }
        public DbSet<Voiture> Voitures { get; set; }
        public DbSet<Entreprise> Entreprises { get; set; }
        public DbSet<Widget> Widgets { get; set; }
        public DbSet<WidgetUtilisateur> WidgetsUtilisateurs { get; set; }
        public DbSet<RendezVous> RendezVous { get; set; }
        public DbSet<Parametrage.Parametrage> Parametrages { get; set; }
        public DbSet<DocumentPieceVente> Documents { get; set; }
        public DbSet<Salarie> Salaries { get; set; }
        public DbSet<SalarieContrat> SalarieContrats { get; set; }
        public DbSet<SalarieIndisponibilite> SalarieIndisponibilites { get; set; }
        public DbSet<SalarieSalaire> SalarieSalaires { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
		}
	}
}
