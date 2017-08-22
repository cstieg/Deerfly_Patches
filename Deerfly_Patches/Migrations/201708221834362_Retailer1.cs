namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Retailer1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Retailers", "Address_AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs");
            DropIndex("dbo.Retailers", new[] { "LatLngId" });
            DropIndex("dbo.Retailers", new[] { "Address_AddressId" });
            RenameColumn(table: "dbo.Retailers", name: "Address_AddressId", newName: "AddressId");
            AlterColumn("dbo.Retailers", "LatLngId", c => c.Int());
            AlterColumn("dbo.Retailers", "AddressId", c => c.Int(nullable: false));
            CreateIndex("dbo.Retailers", "AddressId");
            CreateIndex("dbo.Retailers", "LatLngId");
            AddForeignKey("dbo.Retailers", "AddressId", "dbo.Addresses", "AddressId", cascadeDelete: true);
            AddForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs", "LatLngId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs");
            DropForeignKey("dbo.Retailers", "AddressId", "dbo.Addresses");
            DropIndex("dbo.Retailers", new[] { "LatLngId" });
            DropIndex("dbo.Retailers", new[] { "AddressId" });
            AlterColumn("dbo.Retailers", "AddressId", c => c.Int());
            AlterColumn("dbo.Retailers", "LatLngId", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Retailers", name: "AddressId", newName: "Address_AddressId");
            CreateIndex("dbo.Retailers", "Address_AddressId");
            CreateIndex("dbo.Retailers", "LatLngId");
            AddForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs", "LatLngId", cascadeDelete: true);
            AddForeignKey("dbo.Retailers", "Address_AddressId", "dbo.Addresses", "AddressId");
        }
    }
}
