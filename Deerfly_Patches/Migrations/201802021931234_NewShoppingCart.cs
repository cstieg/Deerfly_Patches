namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewShoppingCart : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.ProductBases", newName: "Products");
            DropPrimaryKey("dbo.Retailers");
            DropPrimaryKey("dbo.Testimonials");
            DropForeignKey("dbo.AddressBases", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.PromoCodes", "ShoppingCart_Id", "dbo.ShoppingCarts");
            DropIndex("dbo.AddressBases", new[] { "CustomerId" });
            DropIndex("dbo.PromoCodes", new[] { "ShoppingCart_Id" });
            RenameColumn(table: "dbo.ShoppingCarts", name: "Order_Id", newName: "OrderId");
            RenameColumn(table: "dbo.Retailers", name: "RetailerId", newName: "Id");
            RenameColumn(table: "dbo.Testimonials", name: "TestimonialId", newName: "Id");
            RenameIndex(table: "dbo.ShoppingCarts", name: "IX_Order_Id", newName: "IX_OrderId");
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(),
                        Recipient = c.String(maxLength: 50),
                        Address1 = c.String(maxLength: 50),
                        Address2 = c.String(maxLength: 50),
                        City = c.String(maxLength: 50),
                        State = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 15),
                        Country = c.String(maxLength: 50),
                        Phone = c.String(maxLength: 25),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.CustomerId);
            
            AddColumn("dbo.OrderDetails", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "Tax", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "NoteToPayee", c => c.String(maxLength: 255));
            AddColumn("dbo.Products", "UrlName", c => c.String());
            AddColumn("dbo.Products", "MetaDescription", c => c.String());
            AddColumn("dbo.ShippingCountries", "BaseShippingIsPerItem", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShippingCountries", "AdditionalShippingIsPerItem", c => c.Boolean(nullable: false));
            AddColumn("dbo.ShippingCountries", "FreeShipping", c => c.Boolean(nullable: false));

            AlterColumn("dbo.AddressBases", "Recipient", c => c.String());
            AlterColumn("dbo.AddressBases", "Address1", c => c.String());
            AlterColumn("dbo.AddressBases", "Address2", c => c.String());
            AlterColumn("dbo.AddressBases", "City", c => c.String());
            AlterColumn("dbo.AddressBases", "State", c => c.String());
            AlterColumn("dbo.AddressBases", "PostalCode", c => c.String());
            AlterColumn("dbo.AddressBases", "Country", c => c.String());
            AlterColumn("dbo.AddressBases", "Phone", c => c.String());
            AddPrimaryKey("dbo.Retailers", "Id");
            AddPrimaryKey("dbo.Testimonials", "Id");
            DropColumn("dbo.AddressBases", "CustomerId");
            DropColumn("dbo.AddressBases", "Discriminator");
            DropColumn("dbo.Products", "Discriminator");
            DropColumn("dbo.PromoCodes", "ShoppingCart_Id");
            DropColumn("dbo.ShoppingCarts", "PayeeEmail");
            
        }
        
        public override void Down()
        {
            AddColumn("dbo.Testimonials", "TestimonialId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.ShoppingCarts", "PayeeEmail", c => c.String());
            AddColumn("dbo.Retailers", "RetailerId", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.PromoCodes", "ShoppingCart_Id", c => c.Int());
            AddColumn("dbo.Products", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AddressBases", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AddressBases", "CustomerId", c => c.Int());
            DropForeignKey("dbo.Addresses", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Addresses", new[] { "CustomerId" });
            DropPrimaryKey("dbo.Testimonials");
            DropPrimaryKey("dbo.Retailers");
            AlterColumn("dbo.AddressBases", "Phone", c => c.String(maxLength: 25));
            AlterColumn("dbo.AddressBases", "Country", c => c.String(maxLength: 50));
            AlterColumn("dbo.AddressBases", "PostalCode", c => c.String(maxLength: 15));
            AlterColumn("dbo.AddressBases", "State", c => c.String(maxLength: 50));
            AlterColumn("dbo.AddressBases", "City", c => c.String(maxLength: 50));
            AlterColumn("dbo.AddressBases", "Address2", c => c.String(maxLength: 50));
            AlterColumn("dbo.AddressBases", "Address1", c => c.String(maxLength: 50));
            AlterColumn("dbo.AddressBases", "Recipient", c => c.String(maxLength: 50));
            DropColumn("dbo.Testimonials", "Id");
            DropColumn("dbo.Retailers", "Id");
            DropColumn("dbo.ShippingCountries", "FreeShipping");
            DropColumn("dbo.ShippingCountries", "AdditionalShippingIsPerItem");
            DropColumn("dbo.ShippingCountries", "BaseShippingIsPerItem");
            DropColumn("dbo.Products", "MetaDescription");
            DropColumn("dbo.Products", "UrlName");
            DropColumn("dbo.Orders", "NoteToPayee");
            DropColumn("dbo.Orders", "Tax");
            DropColumn("dbo.OrderDetails", "Tax");
            DropTable("dbo.Addresses");
            AddPrimaryKey("dbo.Testimonials", "TestimonialId");
            AddPrimaryKey("dbo.Retailers", "RetailerId");
            RenameIndex(table: "dbo.ShoppingCarts", name: "IX_OrderId", newName: "IX_Order_Id");
            RenameColumn(table: "dbo.ShoppingCarts", name: "OrderId", newName: "Order_Id");
            CreateIndex("dbo.PromoCodes", "ShoppingCart_Id");
            CreateIndex("dbo.AddressBases", "CustomerId");
            AddForeignKey("dbo.PromoCodes", "ShoppingCart_Id", "dbo.ShoppingCarts", "Id");
            AddForeignKey("dbo.AddressBases", "CustomerId", "dbo.Customers", "Id");
            RenameTable(name: "dbo.Products", newName: "ProductBases");
        }
    }
}
