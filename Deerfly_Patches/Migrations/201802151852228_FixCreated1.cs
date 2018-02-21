namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCreated1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "Created", c => c.DateTime());
            AddColumn("dbo.ShoppingCarts", "Created", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ShoppingCarts", "Created");
            DropColumn("dbo.Orders", "Created");
        }
    }
}
