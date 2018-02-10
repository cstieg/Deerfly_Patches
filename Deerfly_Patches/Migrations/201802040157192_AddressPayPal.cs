namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddressPayPal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "Status", c => c.String());
            AddColumn("dbo.Addresses", "Type", c => c.String());
            AddColumn("dbo.Addresses", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.Addresses", "Recipient", c => c.String(maxLength: 127));
            AlterColumn("dbo.Addresses", "Address1", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Addresses", "Country", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Addresses", "Phone", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "Phone", c => c.String(maxLength: 25));
            AlterColumn("dbo.Addresses", "Country", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "Address1", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "Recipient", c => c.String(maxLength: 50));
            DropColumn("dbo.Addresses", "Discriminator");
            DropColumn("dbo.Addresses", "Type");
            DropColumn("dbo.Addresses", "Status");
        }
    }
}
