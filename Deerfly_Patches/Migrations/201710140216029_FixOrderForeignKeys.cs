namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixOrderForeignKeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Orders", "BillToAddress_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Orders", "ShipToAddress_AddressId", "dbo.Addresses");
            DropIndex("dbo.Orders", new[] { "BillToAddress_AddressId" });
            DropIndex("dbo.Orders", new[] { "ShipToAddress_AddressId" });
            DropColumn("dbo.Orders", "BillToAddressId");
            DropColumn("dbo.Orders", "ShipToAddressId");
            RenameColumn(table: "dbo.Orders", name: "BillToAddress_AddressId", newName: "BillToAddressId");
            RenameColumn(table: "dbo.Orders", name: "ShipToAddress_AddressId", newName: "ShipToAddressId");
            AlterColumn("dbo.Orders", "BillToAddressId", c => c.Int(nullable: false));
            AlterColumn("dbo.Orders", "ShipToAddressId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "ShipToAddressId");
            CreateIndex("dbo.Orders", "BillToAddressId");
            AddForeignKey("dbo.Orders", "BillToAddressId", "dbo.Addresses", "AddressId", cascadeDelete: false);
            AddForeignKey("dbo.Orders", "ShipToAddressId", "dbo.Addresses", "AddressId", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "ShipToAddressId", "dbo.Addresses");
            DropForeignKey("dbo.Orders", "BillToAddressId", "dbo.Addresses");
            DropIndex("dbo.Orders", new[] { "BillToAddressId" });
            DropIndex("dbo.Orders", new[] { "ShipToAddressId" });
            AlterColumn("dbo.Orders", "ShipToAddressId", c => c.Int());
            AlterColumn("dbo.Orders", "BillToAddressId", c => c.Int());
            RenameColumn(table: "dbo.Orders", name: "ShipToAddressId", newName: "ShipToAddress_AddressId");
            RenameColumn(table: "dbo.Orders", name: "BillToAddressId", newName: "BillToAddress_AddressId");
            AddColumn("dbo.Orders", "ShipToAddressId", c => c.Int(nullable: false));
            AddColumn("dbo.Orders", "BillToAddressId", c => c.Int(nullable: false));
            CreateIndex("dbo.Orders", "ShipToAddress_AddressId");
            CreateIndex("dbo.Orders", "BillToAddress_AddressId");
            AddForeignKey("dbo.Orders", "ShipToAddress_AddressId", "dbo.Addresses", "AddressId");
            AddForeignKey("dbo.Orders", "BillToAddress_AddressId", "dbo.Addresses", "AddressId");
        }
    }
}
