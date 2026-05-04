namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _15 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PieceVentes", "TotalTTC", c => c.Single(nullable: false));
            AddColumn("dbo.PieceVentes", "MontantTVA", c => c.Single(nullable: false));
            AddColumn("dbo.PieceVentes", "TotalHT", c => c.Single(nullable: false));
            AddColumn("dbo.PieceVentes", "BeneficeTTC", c => c.Single(nullable: false));
            AddColumn("dbo.PieceVentes", "ResteAPayer", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PieceVentes", "ResteAPayer");
            DropColumn("dbo.PieceVentes", "BeneficeTTC");
            DropColumn("dbo.PieceVentes", "TotalHT");
            DropColumn("dbo.PieceVentes", "MontantTVA");
            DropColumn("dbo.PieceVentes", "TotalTTC");
        }
    }
}
