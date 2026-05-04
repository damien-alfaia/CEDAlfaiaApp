namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _11 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "EmailSMTP", c => c.String());
            AddColumn("dbo.Parametrages", "PasswordSMTP", c => c.String());
            DropColumn("dbo.PieceVentes", "IsFactureEnvoyeComptable");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PieceVentes", "IsFactureEnvoyeComptable", c => c.Boolean(nullable: false));
            DropColumn("dbo.Parametrages", "PasswordSMTP");
            DropColumn("dbo.Parametrages", "EmailSMTP");
        }
    }
}
