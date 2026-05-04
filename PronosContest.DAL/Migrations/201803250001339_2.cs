namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PieceVentes", "DateDevis", c => c.DateTime());
            AlterColumn("dbo.PieceVentes", "DateFacture", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PieceVentes", "DateFacture", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PieceVentes", "DateDevis", c => c.DateTime(nullable: false));
        }
    }
}
