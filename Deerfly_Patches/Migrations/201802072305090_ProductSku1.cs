namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductSku1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Products", new[] { "Sku" });
            DropIndex("dbo.Products", new[] { "UrlName" });
            CreateIndex("dbo.Products", "Sku");
            CreateIndex("dbo.Products", "UrlName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Products", new[] { "UrlName" });
            DropIndex("dbo.Products", new[] { "Sku" });
            CreateIndex("dbo.Products", "UrlName", unique: true);
            CreateIndex("dbo.Products", "Sku", unique: true);
        }
    }
}
