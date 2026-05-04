namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _14 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "SIRET", c => c.String());
            AddColumn("dbo.Parametrages", "CodeAPE", c => c.String());
            AddColumn("dbo.Parametrages", "TVAIntraCommunautaire", c => c.String());
            AddColumn("dbo.Parametrages", "LibelleBasDePage", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parametrages", "LibelleBasDePage");
            DropColumn("dbo.Parametrages", "TVAIntraCommunautaire");
            DropColumn("dbo.Parametrages", "CodeAPE");
            DropColumn("dbo.Parametrages", "SIRET");
        }
    }
}
