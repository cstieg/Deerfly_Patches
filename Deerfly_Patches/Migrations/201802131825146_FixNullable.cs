namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products");
            DropIndex("dbo.PromoCodes", new[] { "PromotionalItemId" });
            DropIndex("dbo.PromoCodes", new[] { "WithPurchaseOfId" });
            DropIndex("dbo.PromoCodes", new[] { "SpecialPriceItemId" });
            AlterColumn("dbo.PromoCodes", "PromotionalItemId", c => c.Int(nullable: false));
            AlterColumn("dbo.PromoCodes", "WithPurchaseOfId", c => c.Int(nullable: false));
            AlterColumn("dbo.PromoCodes", "SpecialPriceItemId", c => c.Int(nullable: false));
            CreateIndex("dbo.PromoCodes", "PromotionalItemId");
            CreateIndex("dbo.PromoCodes", "WithPurchaseOfId");
            CreateIndex("dbo.PromoCodes", "SpecialPriceItemId");
            AddForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products");
            DropIndex("dbo.PromoCodes", new[] { "SpecialPriceItemId" });
            DropIndex("dbo.PromoCodes", new[] { "WithPurchaseOfId" });
            DropIndex("dbo.PromoCodes", new[] { "PromotionalItemId" });
            AlterColumn("dbo.PromoCodes", "SpecialPriceItemId", c => c.Int());
            AlterColumn("dbo.PromoCodes", "WithPurchaseOfId", c => c.Int());
            AlterColumn("dbo.PromoCodes", "PromotionalItemId", c => c.Int());
            CreateIndex("dbo.PromoCodes", "SpecialPriceItemId");
            CreateIndex("dbo.PromoCodes", "WithPurchaseOfId");
            CreateIndex("dbo.PromoCodes", "PromotionalItemId");
            AddForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products", "Id");
            AddForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products", "Id");
            AddForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products", "Id");
        }
    }
}
