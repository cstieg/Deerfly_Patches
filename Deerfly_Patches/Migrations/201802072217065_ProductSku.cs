namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductSku : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Sku", c => c.String(maxLength: 20));
            AddColumn("dbo.Products", "Gtin", c => c.String(maxLength: 14));
            AlterColumn("dbo.Customers", "EmailAddress", c => c.String(maxLength: 254));
            AlterColumn("dbo.Products", "UrlName", c => c.String(maxLength: 20));
            CreateIndex("dbo.Customers", "EmailAddress", unique: true);
            CreateIndex("dbo.Countries", "IsoCode2", unique: true);
            CreateIndex("dbo.Products", "Sku");
            CreateIndex("dbo.Products", "UrlName");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Products", new[] { "UrlName" });
            DropIndex("dbo.Products", new[] { "Sku" });
            DropIndex("dbo.Countries", new[] { "IsoCode2" });
            DropIndex("dbo.Customers", new[] { "EmailAddress" });
            AlterColumn("dbo.Products", "UrlName", c => c.String());
            AlterColumn("dbo.Customers", "EmailAddress", c => c.String());
            DropColumn("dbo.Products", "Gtin");
            DropColumn("dbo.Products", "Sku");
        }
    }
}
