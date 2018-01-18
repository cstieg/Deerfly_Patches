namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ToModular : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "BillToAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Orders", "ShipToAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Addresses", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.WebImages", "ProductId", "dbo.Products");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products");
            DropForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products");
            DropForeignKey("dbo.Retailers", "AddressId", "dbo.Addresses");
            RenameTable(name: "dbo.Products", newName: "ProductBases");
            RenameTable(name: "dbo.Addresses", newName: "AddressBases");
            DropIndex("dbo.AddressBases", new[] { "Phone" });
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropIndex("dbo.Orders", new[] { "ShipToAddressId" });
            DropIndex("dbo.Orders", new[] { "BillToAddressId" });
            DropPrimaryKey("dbo.Customers");
            DropPrimaryKey("dbo.LatLngs");
            DropPrimaryKey("dbo.OrderDetails");
            DropPrimaryKey("dbo.Orders");
            DropPrimaryKey("dbo.ProductBases");
            DropPrimaryKey("dbo.PromoCodes");
            DropPrimaryKey("dbo.AddressBases");
            CreateTable(
                "dbo.ShippingSchemes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 30),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShippingCountries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShippingSchemeId = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                        MinQty = c.Int(),
                        MaxQty = c.Int(),
                        AdditionalShipping = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .ForeignKey("dbo.ShippingSchemes", t => t.ShippingSchemeId, cascadeDelete: true)
                .Index(t => t.ShippingSchemeId)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        IsoCode2 = c.String(nullable: false, maxLength: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ShoppingCarts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OwnerId = c.String(),
                        Country = c.String(),
                        PayeeEmail = c.String(),
                        Order_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.Order_Id)
                .Index(t => t.Order_Id);
            
            RenameColumn("dbo.Customers", "CustomerId", "Id");
            RenameColumn("dbo.LatLngs", "LatLngId", "Id");
            RenameColumn("dbo.OrderDetails", "OrderDetailId", "Id");
            RenameColumn("dbo.Orders", "OrderId", "Id");
            RenameColumn("dbo.ProductBases", "ProductId", "Id");
            RenameColumn("dbo.PromoCodes", "PromoCodeId", "Id");
            RenameColumn("dbo.AddressBases", "AddressId", "Id");

            AddColumn("dbo.Customers", "FirstName", c => c.String());
            AddColumn("dbo.Customers", "LastName", c => c.String());
            AddColumn("dbo.Orders", "Cart", c => c.String());
            AddColumn("dbo.ProductBases", "ShippingSchemeId", c => c.Int());
            AddColumn("dbo.ProductBases", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.PromoCodes", "ShoppingCart_Id", c => c.Int());
            AddColumn("dbo.AddressBases", "Discriminator", c => c.String(nullable: false, maxLength: 128));

            AlterColumn("dbo.Customers", "CustomerName", c => c.String());
            AlterColumn("dbo.Orders", "CustomerId", c => c.Int());
            AlterColumn("dbo.Orders", "DateOrdered", c => c.DateTime());
            AlterColumn("dbo.Orders", "ShipToAddressId", c => c.Int());
            AlterColumn("dbo.Orders", "BillToAddressId", c => c.Int());
            AddPrimaryKey("dbo.Customers", "Id");
            AddPrimaryKey("dbo.LatLngs", "Id");
            AddPrimaryKey("dbo.OrderDetails", "Id");
            AddPrimaryKey("dbo.Orders", "Id");
            AddPrimaryKey("dbo.ProductBases", "Id");
            AddPrimaryKey("dbo.PromoCodes", "Id");
            AddPrimaryKey("dbo.AddressBases", "Id");
            CreateIndex("dbo.Orders", "CustomerId");
            CreateIndex("dbo.Orders", "ShipToAddressId");
            CreateIndex("dbo.Orders", "BillToAddressId");
            CreateIndex("dbo.ProductBases", "ShippingSchemeId");
            CreateIndex("dbo.PromoCodes", "ShoppingCart_Id");
            AddForeignKey("dbo.ProductBases", "ShippingSchemeId", "dbo.ShippingSchemes", "Id");
            AddForeignKey("dbo.PromoCodes", "ShoppingCart_Id", "dbo.ShoppingCarts", "Id");
            AddForeignKey("dbo.Orders", "BillToAddressId", "dbo.AddressBases", "Id");
            AddForeignKey("dbo.Orders", "CustomerId", "dbo.Customers", "Id");
            AddForeignKey("dbo.Orders", "ShipToAddressId", "dbo.AddressBases", "Id");
            AddForeignKey("dbo.AddressBases", "CustomerId", "dbo.Customers", "Id");
            AddForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs", "Id");
            AddForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
            AddForeignKey("dbo.WebImages", "ProductId", "dbo.ProductBases", "Id");
            AddForeignKey("dbo.OrderDetails", "ProductId", "dbo.ProductBases", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.ProductBases", "Id");
            AddForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.ProductBases", "Id");
            AddForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.ProductBases", "Id");
            AddForeignKey("dbo.Retailers", "AddressId", "dbo.AddressBases", "Id", cascadeDelete: true);
            DropColumn("dbo.AddressBases", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AddressBases", "Type", c => c.Int(nullable: false));
            AddColumn("dbo.AddressBases", "AddressId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.PromoCodes", "PromoCodeId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.ProductBases", "ProductId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Orders", "OrderId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.OrderDetails", "OrderDetailId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.LatLngs", "LatLngId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.Customers", "CustomerId", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.Retailers", "AddressId", "dbo.AddressBases");
            DropForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.ProductBases");
            DropForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.ProductBases");
            DropForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.ProductBases");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.ProductBases");
            DropForeignKey("dbo.WebImages", "ProductId", "dbo.ProductBases");
            DropForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs");
            DropForeignKey("dbo.AddressBases", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Orders", "ShipToAddressId", "dbo.AddressBases");
            DropForeignKey("dbo.Orders", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Orders", "BillToAddressId", "dbo.AddressBases");
            DropForeignKey("dbo.PromoCodes", "ShoppingCart_Id", "dbo.ShoppingCarts");
            DropForeignKey("dbo.ShoppingCarts", "Order_Id", "dbo.Orders");
            DropForeignKey("dbo.ProductBases", "ShippingSchemeId", "dbo.ShippingSchemes");
            DropForeignKey("dbo.ShippingCountries", "ShippingSchemeId", "dbo.ShippingSchemes");
            DropForeignKey("dbo.ShippingCountries", "CountryId", "dbo.Countries");
            DropIndex("dbo.ShoppingCarts", new[] { "Order_Id" });
            DropIndex("dbo.PromoCodes", new[] { "ShoppingCart_Id" });
            DropIndex("dbo.ShippingCountries", new[] { "CountryId" });
            DropIndex("dbo.ShippingCountries", new[] { "ShippingSchemeId" });
            DropIndex("dbo.ProductBases", new[] { "ShippingSchemeId" });
            DropIndex("dbo.Orders", new[] { "BillToAddressId" });
            DropIndex("dbo.Orders", new[] { "ShipToAddressId" });
            DropIndex("dbo.Orders", new[] { "CustomerId" });
            DropPrimaryKey("dbo.AddressBases");
            DropPrimaryKey("dbo.PromoCodes");
            DropPrimaryKey("dbo.ProductBases");
            DropPrimaryKey("dbo.Orders");
            DropPrimaryKey("dbo.OrderDetails");
            DropPrimaryKey("dbo.LatLngs");
            DropPrimaryKey("dbo.Customers");
            AlterColumn("dbo.Orders", "BillToAddressId", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "ShipToAddressId", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "DateOrdered", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "CustomerId", c => c.Int(nullable: false));
            AlterColumn("dbo.Customers", "CustomerName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.AddressBases", "Discriminator");
            DropColumn("dbo.AddressBases", "Id");
            DropColumn("dbo.PromoCodes", "ShoppingCart_Id");
            DropColumn("dbo.PromoCodes", "Id");
            DropColumn("dbo.ProductBases", "Discriminator");
            DropColumn("dbo.ProductBases", "ShippingSchemeId");
            DropColumn("dbo.ProductBases", "Id");
            DropColumn("dbo.Orders", "Cart");
            DropColumn("dbo.Orders", "Id");
            DropColumn("dbo.OrderDetails", "Id");
            DropColumn("dbo.LatLngs", "Id");
            DropColumn("dbo.Customers", "LastName");
            DropColumn("dbo.Customers", "FirstName");
            DropColumn("dbo.Customers", "Id");
            DropTable("dbo.ShoppingCarts");
            DropTable("dbo.Countries");
            DropTable("dbo.ShippingCountries");
            DropTable("dbo.ShippingSchemes");
            AddPrimaryKey("dbo.AddressBases", "AddressId");
            AddPrimaryKey("dbo.PromoCodes", "PromoCodeId");
            AddPrimaryKey("dbo.ProductBases", "ProductId");
            AddPrimaryKey("dbo.Orders", "OrderId");
            AddPrimaryKey("dbo.OrderDetails", "OrderDetailId");
            AddPrimaryKey("dbo.LatLngs", "LatLngId");
            AddPrimaryKey("dbo.Customers", "CustomerId");
            CreateIndex("dbo.Orders", "BillToAddressId");
            CreateIndex("dbo.Orders", "ShipToAddressId");
            CreateIndex("dbo.Orders", "CustomerId");
            CreateIndex("dbo.AddressBases", "Phone");
            AddForeignKey("dbo.Retailers", "AddressId", "dbo.Addresses", "AddressId", cascadeDelete: true);
            AddForeignKey("dbo.PromoCodes", "WithPurchaseOfId", "dbo.Products", "ProductId");
            AddForeignKey("dbo.PromoCodes", "SpecialPriceItemId", "dbo.Products", "ProductId");
            AddForeignKey("dbo.PromoCodes", "PromotionalItemId", "dbo.Products", "ProductId");
            AddForeignKey("dbo.OrderDetails", "ProductId", "dbo.Products", "ProductId", cascadeDelete: true);
            AddForeignKey("dbo.WebImages", "ProductId", "dbo.Products", "ProductId");
            AddForeignKey("dbo.OrderDetails", "OrderId", "dbo.Orders", "OrderId", cascadeDelete: true);
            AddForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs", "LatLngId");
            AddForeignKey("dbo.Addresses", "CustomerId", "dbo.Customers", "CustomerId");
            AddForeignKey("dbo.Orders", "ShipToAddressId", "dbo.Addresses", "AddressId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "CustomerId", "dbo.Customers", "CustomerId", cascadeDelete: true);
            AddForeignKey("dbo.Orders", "BillToAddressId", "dbo.Addresses", "AddressId", cascadeDelete: true);
            RenameTable(name: "dbo.AddressBases", newName: "Addresses");
            RenameTable(name: "dbo.ProductBases", newName: "Products");
        }
    }
}
