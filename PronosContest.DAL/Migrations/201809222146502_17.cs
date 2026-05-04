namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _17 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clients", "Code", c => c.String(maxLength: 256, unicode: false));
            AddColumn("dbo.Clients", "IsProspect", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clients", "IsProspect");
            DropColumn("dbo.Clients", "Code");
        }
    }
}
