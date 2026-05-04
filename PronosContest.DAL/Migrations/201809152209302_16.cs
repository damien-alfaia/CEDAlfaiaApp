namespace CEDAlfaiaApp.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _16 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WidgetUtilisateurs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        WidgetID = c.Int(),
                        CompteUtilisateurID = c.Int(),
                        Titre = c.String(),
                        HtmlOwnerZoneUID = c.String(),
                        HtmlIndex = c.Int(),
                        HtmlTop = c.String(),
                        HtmlLeft = c.String(),
                        DateDebut = c.DateTime(),
                        DateFin = c.DateTime(),
                        IsMoisEnCours = c.Boolean(),
                        IsAnneeEnCours = c.Boolean(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.CompteUtilisateurs", t => t.CompteUtilisateurID)
                .ForeignKey("dbo.Widgets", t => t.WidgetID)
                .Index(t => t.WidgetID)
                .Index(t => t.CompteUtilisateurID);
            
            CreateTable(
                "dbo.Widgets",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Libelle = c.String(),
                        HtmlName = c.String(),
                        HtmlPanelUID = c.String(),
                        HtmlPartialWidget = c.String(),
                        HtmlWidth = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WidgetUtilisateurs", "WidgetID", "dbo.Widgets");
            DropForeignKey("dbo.WidgetUtilisateurs", "CompteUtilisateurID", "dbo.CompteUtilisateurs");
            DropIndex("dbo.WidgetUtilisateurs", new[] { "CompteUtilisateurID" });
            DropIndex("dbo.WidgetUtilisateurs", new[] { "WidgetID" });
            DropTable("dbo.Widgets");
            DropTable("dbo.WidgetUtilisateurs");
        }
    }
}
