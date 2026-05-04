namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "EmailEntreprise", c => c.String());
            AddColumn("dbo.Parametrages", "TelephoneEntreprise", c => c.String());
            AddColumn("dbo.Parametrages", "PortableEntreprise", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Parametrages", "PortableEntreprise");
            DropColumn("dbo.Parametrages", "TelephoneEntreprise");
            DropColumn("dbo.Parametrages", "EmailEntreprise");
        }
    }
}
