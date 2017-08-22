namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Retailer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LatLngs",
                c => new
                    {
                        LatLngId = c.Int(nullable: false, identity: true),
                        Lat = c.Single(nullable: false),
                        Lng = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.LatLngId);
            
            CreateTable(
                "dbo.Retailers",
                c => new
                    {
                        RetailerId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        LatLngId = c.Int(nullable: false),
                        Website = c.String(),
                        Address_AddressId = c.Int(),
                    })
                .PrimaryKey(t => t.RetailerId)
                .ForeignKey("dbo.Addresses", t => t.Address_AddressId)
                .ForeignKey("dbo.LatLngs", t => t.LatLngId, cascadeDelete: true)
                .Index(t => t.LatLngId)
                .Index(t => t.Address_AddressId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Retailers", "LatLngId", "dbo.LatLngs");
            DropForeignKey("dbo.Retailers", "Address_AddressId", "dbo.Addresses");
            DropIndex("dbo.Retailers", new[] { "Address_AddressId" });
            DropIndex("dbo.Retailers", new[] { "LatLngId" });
            DropTable("dbo.Retailers");
            DropTable("dbo.LatLngs");
        }
    }
}
