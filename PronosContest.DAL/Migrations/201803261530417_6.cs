namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "MainDOeuvreMontantHoraire", c => c.Single(nullable: false));
            AddColumn("dbo.PieceVentes", "MainDOeuvreDuree", c => c.Int(nullable: false));
            AddColumn("dbo.PieceVentes", "MainDOeuvreMontantHoraire", c => c.Single(nullable: false));
            DropColumn("dbo.PieceVentes", "MainDOeuvre");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PieceVentes", "MainDOeuvre", c => c.Single(nullable: false));
            DropColumn("dbo.PieceVentes", "MainDOeuvreMontantHoraire");
            DropColumn("dbo.PieceVentes", "MainDOeuvreDuree");
            DropColumn("dbo.Parametrages", "MainDOeuvreMontantHoraire");
        }
    }
}
