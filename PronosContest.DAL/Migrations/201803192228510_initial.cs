namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false, maxLength: 256, unicode: false),
                        Prenom = c.String(maxLength: 256, unicode: false),
                        Adresse_Ligne1 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne2 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne3 = c.String(maxLength: 50, unicode: false),
                        Adresse_CodePostal = c.String(maxLength: 50, unicode: false),
                        Adresse_Ville = c.String(maxLength: 50, unicode: false),
                        Adresse_Pays = c.String(maxLength: 50, unicode: false),
                        InformationsComplementaires = c.String(maxLength: 5000, unicode: false),
                        Telephone = c.String(maxLength: 100, unicode: false),
                        Email = c.String(maxLength: 256, unicode: false),
                        DateSuppression = c.DateTime(),
                        EntrepriseID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Entreprises", t => t.EntrepriseID)
                .Index(t => t.EntrepriseID);
            
            CreateTable(
                "dbo.Entreprises",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false, maxLength: 256, unicode: false),
                        Adresse_Ligne1 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne2 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne3 = c.String(maxLength: 50, unicode: false),
                        Adresse_CodePostal = c.String(maxLength: 50, unicode: false),
                        Adresse_Ville = c.String(maxLength: 50, unicode: false),
                        Adresse_Pays = c.String(maxLength: 50, unicode: false),
                        Siren = c.String(),
                        Siret = c.String(),
                        Commentaire = c.String(maxLength: 5000, unicode: false),
                        DateCreation = c.DateTime(nullable: false),
                        IsActif = c.Boolean(nullable: false),
                        ParametrageID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Parametrages", t => t.ParametrageID)
                .Index(t => t.ParametrageID);
            
            CreateTable(
                "dbo.Parametrages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        NomEntreprise = c.String(),
                        Adresse_Ligne1 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne2 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne3 = c.String(maxLength: 50, unicode: false),
                        Adresse_CodePostal = c.String(maxLength: 50, unicode: false),
                        Adresse_Ville = c.String(maxLength: 50, unicode: false),
                        Adresse_Pays = c.String(maxLength: 50, unicode: false),
                        Logo = c.Binary(),
                        TVA = c.Single(nullable: false),
                        EmailComptable = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.CompteUtilisateurs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 256, unicode: false),
                        Prenom = c.String(maxLength: 256, unicode: false),
                        Nom = c.String(),
                        Role = c.Int(nullable: false),
                        Password = c.Binary(nullable: false),
                        Adresse_Ligne1 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne2 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne3 = c.String(maxLength: 50, unicode: false),
                        Adresse_CodePostal = c.String(maxLength: 50, unicode: false),
                        Adresse_Ville = c.String(maxLength: 50, unicode: false),
                        Adresse_Pays = c.String(maxLength: 50, unicode: false),
                        EntrepriseID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Entreprises", t => t.EntrepriseID)
                .Index(t => t.EntrepriseID);
            
            CreateTable(
                "dbo.Voitures",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Immatriculation = c.String(maxLength: 20, unicode: false),
                        ModeleID = c.Int(),
                        ClientID = c.Int(),
                        IsPrincipale = c.Boolean(nullable: false),
                        DateSuppression = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clients", t => t.ClientID)
                .ForeignKey("dbo.Modeles", t => t.ModeleID)
                .Index(t => t.ModeleID)
                .Index(t => t.ClientID);
            
            CreateTable(
                "dbo.Modeles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        MarqueID = c.Int(),
                        Libelle = c.String(nullable: false, maxLength: 500, unicode: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Marques", t => t.MarqueID)
                .Index(t => t.MarqueID);
            
            CreateTable(
                "dbo.Marques",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 5, unicode: false),
                        Libelle = c.String(nullable: false, maxLength: 500, unicode: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Fournisseurs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nom = c.String(nullable: false, maxLength: 256, unicode: false),
                        Adresse_Ligne1 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne2 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne3 = c.String(maxLength: 50, unicode: false),
                        Adresse_CodePostal = c.String(maxLength: 50, unicode: false),
                        Adresse_Ville = c.String(maxLength: 50, unicode: false),
                        Adresse_Pays = c.String(maxLength: 50, unicode: false),
                        Commentaire = c.String(nullable: false, maxLength: 5000, unicode: false),
                        DateSuppression = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PieceVenteLignes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PieceVenteID = c.Int(),
                        FournisseurID = c.Int(),
                        Libelle = c.String(maxLength: 500, unicode: false),
                        Remise = c.Single(nullable: false),
                        PrixGarageHT = c.Single(nullable: false),
                        PrixGarageTTC = c.Single(nullable: false),
                        PrixClientHT = c.Single(nullable: false),
                        PrixClientTTC = c.Single(nullable: false),
                        Quantite = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Fournisseurs", t => t.FournisseurID)
                .ForeignKey("dbo.PieceVentes", t => t.PieceVenteID)
                .Index(t => t.PieceVenteID)
                .Index(t => t.FournisseurID);
            
            CreateTable(
                "dbo.PieceVentes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(),
                        VoitureID = c.Int(),
                        RendezVousID = c.Int(),
                        DateDevis = c.DateTime(nullable: false),
                        NumDevis = c.Int(),
                        MainDOeuvre = c.Single(nullable: false),
                        DevisFournisseur = c.Binary(),
                        DevisFournisseurFormatFichierBase64 = c.String(),
                        BonLivraison = c.Binary(),
                        BonLivraisonFormatFichierBase64 = c.String(),
                        DateFacture = c.DateTime(nullable: false),
                        NumFacture = c.Int(),
                        Kilometrage = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Clients", t => t.ClientID)
                .ForeignKey("dbo.RendezVous", t => t.RendezVousID)
                .ForeignKey("dbo.Voitures", t => t.VoitureID)
                .Index(t => t.ClientID)
                .Index(t => t.VoitureID)
                .Index(t => t.RendezVousID);
            
            CreateTable(
                "dbo.RendezVous",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Sujet = c.String(),
                        DateHeureDebut = c.DateTime(nullable: false),
                        DateHeureFin = c.DateTime(nullable: false),
                        Duree = c.Int(nullable: false),
                        Commentaire = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PieceVenteLignes", "PieceVenteID", "dbo.PieceVentes");
            DropForeignKey("dbo.PieceVentes", "VoitureID", "dbo.Voitures");
            DropForeignKey("dbo.PieceVentes", "RendezVousID", "dbo.RendezVous");
            DropForeignKey("dbo.PieceVentes", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.PieceVenteLignes", "FournisseurID", "dbo.Fournisseurs");
            DropForeignKey("dbo.Voitures", "ModeleID", "dbo.Modeles");
            DropForeignKey("dbo.Modeles", "MarqueID", "dbo.Marques");
            DropForeignKey("dbo.Voitures", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.Clients", "EntrepriseID", "dbo.Entreprises");
            DropForeignKey("dbo.CompteUtilisateurs", "EntrepriseID", "dbo.Entreprises");
            DropForeignKey("dbo.Entreprises", "ParametrageID", "dbo.Parametrages");
            DropIndex("dbo.PieceVentes", new[] { "RendezVousID" });
            DropIndex("dbo.PieceVentes", new[] { "VoitureID" });
            DropIndex("dbo.PieceVentes", new[] { "ClientID" });
            DropIndex("dbo.PieceVenteLignes", new[] { "FournisseurID" });
            DropIndex("dbo.PieceVenteLignes", new[] { "PieceVenteID" });
            DropIndex("dbo.Modeles", new[] { "MarqueID" });
            DropIndex("dbo.Voitures", new[] { "ClientID" });
            DropIndex("dbo.Voitures", new[] { "ModeleID" });
            DropIndex("dbo.CompteUtilisateurs", new[] { "EntrepriseID" });
            DropIndex("dbo.Entreprises", new[] { "ParametrageID" });
            DropIndex("dbo.Clients", new[] { "EntrepriseID" });
            DropTable("dbo.RendezVous");
            DropTable("dbo.PieceVentes");
            DropTable("dbo.PieceVenteLignes");
            DropTable("dbo.Fournisseurs");
            DropTable("dbo.Marques");
            DropTable("dbo.Modeles");
            DropTable("dbo.Voitures");
            DropTable("dbo.CompteUtilisateurs");
            DropTable("dbo.Parametrages");
            DropTable("dbo.Entreprises");
            DropTable("dbo.Clients");
        }
    }
}
