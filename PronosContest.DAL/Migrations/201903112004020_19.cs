namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _19 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "ObjectifAnnuel", c => c.Single());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parametrages", "ObjectifAnnuel");
        }
    }
}
