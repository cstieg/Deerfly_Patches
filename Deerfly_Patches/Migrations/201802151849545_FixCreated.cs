namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCreated : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "Created");
            DropColumn("dbo.ShoppingCarts", "Created");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ShoppingCarts", "Created", c => c.DateTime(nullable: false));
            AddColumn("dbo.Orders", "Created", c => c.DateTime());
        }
    }
}
