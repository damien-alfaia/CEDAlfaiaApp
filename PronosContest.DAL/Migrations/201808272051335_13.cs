namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _13 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PieceVenteServices",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        PieceVenteID = c.Int(),
                        Libelle = c.String(maxLength: 500, unicode: false),
                        PrixClientHT = c.Single(nullable: false),
                        PrixClientTTC = c.Single(nullable: false),
                        Quantite = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.PieceVentes", t => t.PieceVenteID)
                .Index(t => t.PieceVenteID);
            
            AddColumn("dbo.Clients", "Remise", c => c.Single());
            AddColumn("dbo.PieceVentes", "Remise", c => c.Single());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PieceVenteServices", "PieceVenteID", "dbo.PieceVentes");
            DropIndex("dbo.PieceVenteServices", new[] { "PieceVenteID" });
            DropColumn("dbo.PieceVentes", "Remise");
            DropColumn("dbo.Clients", "Remise");
            DropTable("dbo.PieceVenteServices");
        }
    }
}
