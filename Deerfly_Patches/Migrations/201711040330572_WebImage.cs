namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebImage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WebImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(),
                        ImageUrl = c.String(nullable: false, maxLength: 200),
                        ImageSrcSet = c.String(maxLength: 1000),
                        Caption = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.Products", "ProductInfo", c => c.String(maxLength: 2000));
            DropColumn("dbo.Products", "Description");
            DropColumn("dbo.Products", "ImageUrl");
            DropColumn("dbo.Products", "ImageSrcSet");
            DropColumn("dbo.Products", "Category");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Category", c => c.String(maxLength: 50));
            AddColumn("dbo.Products", "ImageSrcSet", c => c.String());
            AddColumn("dbo.Products", "ImageUrl", c => c.String());
            AddColumn("dbo.Products", "Description", c => c.String(maxLength: 100));
            DropForeignKey("dbo.WebImages", "ProductId", "dbo.Products");
            DropIndex("dbo.WebImages", new[] { "ProductId" });
            DropColumn("dbo.Products", "ProductInfo");
            DropTable("dbo.WebImages");
        }
    }
}
