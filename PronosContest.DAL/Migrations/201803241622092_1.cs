namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DocumentPieceVentes",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PieceVenteID = c.Int(),
                        Libelle = c.String(),
                        TypeDocument = c.Int(nullable: false),
                        Doc = c.Binary(),
                        DocFormatFichierBase64 = c.String(),
                        DateCreation = c.DateTime(nullable: false),
                        DateModification = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PieceVentes", t => t.PieceVenteID)
                .Index(t => t.PieceVenteID);
            
            CreateTable(
                "dbo.PieceVentePaiements",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PieceVenteID = c.Int(),
                        TypePaiement = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        Montant = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PieceVentes", t => t.PieceVenteID)
                .Index(t => t.PieceVenteID);
            
            DropColumn("dbo.PieceVentes", "DevisFournisseur");
            DropColumn("dbo.PieceVentes", "DevisFournisseurFormatFichierBase64");
            DropColumn("dbo.PieceVentes", "BonLivraison");
            DropColumn("dbo.PieceVentes", "BonLivraisonFormatFichierBase64");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PieceVentes", "BonLivraisonFormatFichierBase64", c => c.String());
            AddColumn("dbo.PieceVentes", "BonLivraison", c => c.Binary());
            AddColumn("dbo.PieceVentes", "DevisFournisseurFormatFichierBase64", c => c.String());
            AddColumn("dbo.PieceVentes", "DevisFournisseur", c => c.Binary());
            DropForeignKey("dbo.DocumentPieceVentes", "PieceVenteID", "dbo.PieceVentes");
            DropForeignKey("dbo.PieceVentePaiements", "PieceVenteID", "dbo.PieceVentes");
            DropIndex("dbo.PieceVentePaiements", new[] { "PieceVenteID" });
            DropIndex("dbo.DocumentPieceVentes", new[] { "PieceVenteID" });
            DropTable("dbo.PieceVentePaiements");
            DropTable("dbo.DocumentPieceVentes");
        }
    }
}
