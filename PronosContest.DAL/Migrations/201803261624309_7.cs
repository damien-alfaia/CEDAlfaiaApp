namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RendezVous", "DateHeureDebut", c => c.DateTime());
            AlterColumn("dbo.RendezVous", "DateHeureFin", c => c.DateTime());
            AlterColumn("dbo.RendezVous", "Duree", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RendezVous", "Duree", c => c.Int(nullable: false));
            AlterColumn("dbo.RendezVous", "DateHeureFin", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RendezVous", "DateHeureDebut", c => c.DateTime(nullable: false));
        }
    }
}
