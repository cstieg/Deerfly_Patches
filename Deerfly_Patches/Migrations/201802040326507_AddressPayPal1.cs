namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressPayPal1 : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Orders", new[] { "Cart" });
            AlterColumn("dbo.Addresses", "Recipient", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "Address1", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "Country", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "Phone", c => c.String(maxLength: 25));
            AlterColumn("dbo.Orders", "Cart", c => c.String(maxLength: 30));
            CreateIndex("dbo.Orders", "Cart");
            DropColumn("dbo.Addresses", "Status");
            DropColumn("dbo.Addresses", "Type");
            DropColumn("dbo.Addresses", "Discriminator");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Addresses", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.Addresses", "Type", c => c.String());
            AddColumn("dbo.Addresses", "Status", c => c.String());
            DropIndex("dbo.Orders", new[] { "Cart" });
            AlterColumn("dbo.Orders", "Cart", c => c.String(maxLength: 20));
            AlterColumn("dbo.Addresses", "Phone", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "Country", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Addresses", "Address1", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Addresses", "Recipient", c => c.String(maxLength: 127));
            CreateIndex("dbo.Orders", "Cart");
        }
    }
}
