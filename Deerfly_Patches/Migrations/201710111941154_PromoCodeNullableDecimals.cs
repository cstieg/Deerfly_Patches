namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoCodeNullableDecimals : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PromoCodes", "PromotionalItemPrice", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "MinimumQualifyingPurchase", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "PercentOffOrder", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "PercentOffItem", c => c.Decimal(precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "SpecialPrice", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PromoCodes", "SpecialPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "PercentOffItem", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "PercentOffOrder", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "MinimumQualifyingPurchase", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.PromoCodes", "PromotionalItemPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
