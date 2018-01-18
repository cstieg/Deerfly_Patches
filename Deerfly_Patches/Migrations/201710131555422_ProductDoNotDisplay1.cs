namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductDoNotDisplay1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "PostalCode", c => c.String(maxLength: 15));
            DropColumn("dbo.Addresses", "Zip");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Addresses", "Zip", c => c.String(maxLength: 15));
            DropColumn("dbo.Addresses", "PostalCode");
        }
    }
}
