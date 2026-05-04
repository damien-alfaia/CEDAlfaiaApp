namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PieceVentes", "IsFactureAnnule", c => c.Boolean(nullable: false));
            AddColumn("dbo.PieceVentes", "DateHeureAnnulation", c => c.DateTime(nullable: false));
            AddColumn("dbo.PieceVentes", "CommentaireAnnulation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PieceVentes", "CommentaireAnnulation");
            DropColumn("dbo.PieceVentes", "DateHeureAnnulation");
            DropColumn("dbo.PieceVentes", "IsFactureAnnule");
        }
    }
}
