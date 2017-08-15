namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        CustomerId = c.Int(nullable: false),
                        Recipient = c.String(nullable: false, maxLength: 50),
                        Address1 = c.String(nullable: false, maxLength: 50),
                        Address2 = c.String(maxLength: 50),
                        City = c.String(nullable: false, maxLength: 50),
                        State = c.String(nullable: false, maxLength: 50),
                        Zip = c.String(nullable: false, maxLength: 15),
                        Country = c.String(nullable: false, maxLength: 50),
                        Phone = c.String(maxLength: 25),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        CustomerName = c.String(nullable: false, maxLength: 50),
                        Registered = c.DateTime(nullable: false),
                        LastVisited = c.DateTime(nullable: false),
                        TimesVisited = c.Int(nullable: false),
                        EmailAddress = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Addresses", "CustomerId", "dbo.Customers");
            DropIndex("dbo.Addresses", new[] { "CustomerId" });
            DropTable("dbo.Customers");
            DropTable("dbo.Addresses");
        }
    }
}
