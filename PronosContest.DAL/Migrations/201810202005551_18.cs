namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _18 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SalarieContrats",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(),
                        TypeContrat = c.String(),
                        SalarieID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Salaries", t => t.SalarieID)
                .Index(t => t.SalarieID);
            
            CreateTable(
                "dbo.SalarieIndisponibilites",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(nullable: false),
                        TypeIndisponibilite = c.Int(nullable: false),
                        Motif = c.String(),
                        SalarieContratID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SalarieContrats", t => t.SalarieContratID)
                .Index(t => t.SalarieContratID);
            
            CreateTable(
                "dbo.SalarieSalaires",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        DateDebut = c.DateTime(nullable: false),
                        DateFin = c.DateTime(nullable: false),
                        DatePaiement = c.DateTime(nullable: false),
                        SalaireNet = c.Single(nullable: false),
                        SalarieContratID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.SalarieContrats", t => t.SalarieContratID)
                .Index(t => t.SalarieContratID);
            
            CreateTable(
                "dbo.Salaries",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Nom = c.String(),
                        Prenom = c.String(),
                        DateNaissance = c.DateTime(nullable: false),
                        Adresse_Ligne1 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne2 = c.String(maxLength: 50, unicode: false),
                        Adresse_Ligne3 = c.String(maxLength: 50, unicode: false),
                        Adresse_CodePostal = c.String(maxLength: 50, unicode: false),
                        Adresse_Ville = c.String(maxLength: 50, unicode: false),
                        Adresse_Pays = c.String(maxLength: 50, unicode: false),
                        Telephone = c.String(),
                        Portable = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SalarieContrats", "SalarieID", "dbo.Salaries");
            DropForeignKey("dbo.SalarieSalaires", "SalarieContratID", "dbo.SalarieContrats");
            DropForeignKey("dbo.SalarieIndisponibilites", "SalarieContratID", "dbo.SalarieContrats");
            DropIndex("dbo.SalarieSalaires", new[] { "SalarieContratID" });
            DropIndex("dbo.SalarieIndisponibilites", new[] { "SalarieContratID" });
            DropIndex("dbo.SalarieContrats", new[] { "SalarieID" });
            DropTable("dbo.Salaries");
            DropTable("dbo.SalarieSalaires");
            DropTable("dbo.SalarieIndisponibilites");
            DropTable("dbo.SalarieContrats");
        }
    }
}
