namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class models : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        PlacedInCart = c.DateTime(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CheckedOut = c.Boolean(nullable: false),
                        OrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderDetailId)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ProductId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        DateOrdered = c.DateTime(nullable: false),
                        ShipToAddressId = c.Int(nullable: false),
                        BillToAddressId = c.Int(nullable: false),
                        Subtotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        BillToAddress_AddressId = c.Int(),
                        ShipToAddress_AddressId = c.Int(),
                    })
                .PrimaryKey(t => t.OrderId)
                .ForeignKey("dbo.Addresses", t => t.BillToAddress_AddressId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .ForeignKey("dbo.Addresses", t => t.ShipToAddress_AddressId)
                .Index(t => t.CustomerId)
                .Index(t => t.BillToAddress_AddressId)
                .Index(t => t.ShipToAddress_AddressId);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 100),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Shipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImageURL = c.String(maxLength: 255),
                        Category = c.String(maxLength: 50),
                    })
                .PrimaryKey(t => t.ProductId);
            
            CreateTable(
                "dbo.PromoCodes",
                c => new
                    {
                        PromoCodeId = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 20),
                        Description = c.String(maxLength: 100),
                        PromotionalItemId = c.Int(nullable: true),
                        PromotionalItemPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WithPurchaseOfId = c.Int(nullable: true),
                        MinimumQualifyingPurchase = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PercentOffItem = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PercentOffOrder = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SpecialPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CodeStart = c.DateTime(nullable: false),
                        CodeEnd = c.DateTime(nullable: false),
                        SpecialPriceItem_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.PromoCodeId)
                .ForeignKey("dbo.Products", t => t.PromotionalItemId, cascadeDelete: false)
                .ForeignKey("dbo.Products", t => t.SpecialPriceItem_ProductId)
                .ForeignKey("dbo.Products", t => t.WithPurchaseOfId, cascadeDelete: false)
                .Index(t => t.Code, unique: true)
                .Index(t => t.PromotionalItemId)
                .Index(t => t.WithPurchaseOfId)
                .Index(t => t.SpecialPriceItem_ProductId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItem_ProductId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Orders", "ShipToAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Orders", "BillToAddress_AddressId", "dbo.Addresses");
            DropIndex("dbo.PromoCodes", new[] { "SpecialPriceItem_ProductId" });
            DropIndex("dbo.PromoCodes", new[] { "WithPurchaseOfId" });
            DropIndex("dbo.PromoCodes", new[] { "PromotionalItemId" });
            DropIndex("dbo.PromoCodes", new[] { "Code" });
            DropIndex("dbo.Orders", new[] { "ShipToAddress_AddressId" });
            DropIndex("dbo.Orders", new[] { "BillToAddress_AddressId" });
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderId" });
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropTable("dbo.PromoCodes");
            DropTable("dbo.Products");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderDetails");
        }
    }
}
