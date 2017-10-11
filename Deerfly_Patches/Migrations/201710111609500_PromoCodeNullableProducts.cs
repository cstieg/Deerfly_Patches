namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoCodeNullableProducts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItem_ProductId", "dbo.Products");
            DropIndex("dbo.PromoCodes", new[] { "SpecialPriceItem_ProductId" });
            RenameColumn(table: "dbo.PromoCodes", name: "SpecialPriceItem_ProductId", newName: "SpecialPriceItemId");
            AlterColumn("dbo.PromoCodes", "SpecialPriceItemId", c => c.Int(nullable: false));
            CreateIndex("dbo.PromoCodes", "SpecialPriceItemId");
            AddForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products", "ProductId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products");
            DropIndex("dbo.PromoCodes", new[] { "SpecialPriceItemId" });
            AlterColumn("dbo.PromoCodes", "SpecialPriceItemId", c => c.Int());
            RenameColumn(table: "dbo.PromoCodes", name: "SpecialPriceItemId", newName: "SpecialPriceItem_ProductId");
            CreateIndex("dbo.PromoCodes", "SpecialPriceItem_ProductId");
            AddForeignKey("dbo.PromoCodes", "SpecialPriceItem_ProductId", "dbo.Products", "ProductId");
        }
    }
}
