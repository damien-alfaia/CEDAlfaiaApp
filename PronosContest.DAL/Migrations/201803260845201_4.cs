namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "IsSavePieceDeVente", c => c.Boolean(nullable: false));
            AddColumn("dbo.PieceVentes", "IsFactureEnvoyeComptable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PieceVentes", "IsFactureEnvoyeComptable");
            DropColumn("dbo.Parametrages", "IsSavePieceDeVente");
        }
    }
}
