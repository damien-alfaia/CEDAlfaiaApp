namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PieceVentes", "DateHeureAnnulation", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PieceVentes", "DateHeureAnnulation", c => c.DateTime(nullable: false));
        }
    }
}
