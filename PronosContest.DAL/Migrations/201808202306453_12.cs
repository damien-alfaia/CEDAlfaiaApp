namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "Entete", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parametrages", "Entete");
        }
    }
}
