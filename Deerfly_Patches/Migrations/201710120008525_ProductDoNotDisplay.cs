namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductDoNotDisplay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DoNotDisplay", c => c.Boolean(nullable: false));
            DropColumn("dbo.Products", "PayPalUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "PayPalUrl", c => c.String());
            DropColumn("dbo.Products", "DoNotDisplay");
        }
    }
}
