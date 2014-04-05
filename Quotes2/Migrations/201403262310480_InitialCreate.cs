namespace Quotes2.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.Quotations",
                c => new
                    {
                        QuotationID = c.Int(nullable: false, identity: true),
                        Quote = c.String(),
                        Author = c.String(),
                        Date = c.DateTime(nullable: false),
                        CategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuotationID)
                .ForeignKey("dbo.Categories", t => t.CategoryID, cascadeDelete: true)
                .Index(t => t.CategoryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Quotations", "CategoryID", "dbo.Categories");
            DropIndex("dbo.Quotations", new[] { "CategoryID" });
            DropTable("dbo.Quotations");
            DropTable("dbo.Categories");
        }
    }
}
