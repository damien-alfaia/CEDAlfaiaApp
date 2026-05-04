namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _10 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Parametrages", "EmailEnvoiSMTP", c => c.String());
            AddColumn("dbo.Parametrages", "ProtocoleSMTP", c => c.String());
            AddColumn("dbo.Parametrages", "PortSMTP", c => c.Int());
            AddColumn("dbo.Parametrages", "ServeurSMTP", c => c.String());
            AddColumn("dbo.Parametrages", "IsSSL", c => c.Boolean(nullable: false));
            AddColumn("dbo.PieceVentes", "IsValide", c => c.Boolean(nullable: false));
            AddColumn("dbo.PieceVentes", "DateHeureValidation", c => c.DateTime());
            AddColumn("dbo.PieceVentes", "IsEnvoyeComptable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PieceVentes", "IsEnvoyeComptable");
            DropColumn("dbo.PieceVentes", "DateHeureValidation");
            DropColumn("dbo.PieceVentes", "IsValide");
            DropColumn("dbo.Parametrages", "IsSSL");
            DropColumn("dbo.Parametrages", "ServeurSMTP");
            DropColumn("dbo.Parametrages", "PortSMTP");
            DropColumn("dbo.Parametrages", "ProtocoleSMTP");
            DropColumn("dbo.Parametrages", "EmailEnvoiSMTP");
        }
    }
}
