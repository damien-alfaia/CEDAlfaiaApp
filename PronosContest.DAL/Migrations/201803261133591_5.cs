namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DocumentPieceVentes", "DateCreation", c => c.DateTime());
            AlterColumn("dbo.DocumentPieceVentes", "DateModification", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DocumentPieceVentes", "DateModification", c => c.DateTime(nullable: false));
            AlterColumn("dbo.DocumentPieceVentes", "DateCreation", c => c.DateTime(nullable: false));
        }
    }
}
