namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NoProductExtension : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Products", "ProductExtensionId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "ProductExtensionId", c => c.Int());
        }
    }
}
