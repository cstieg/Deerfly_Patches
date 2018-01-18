namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Retailer2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Addresses", "Recipient", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "Recipient", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
