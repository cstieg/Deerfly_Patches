namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProductExtension : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ProductExtensionId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ProductExtensionId");
        }
    }
}
