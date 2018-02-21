namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixCreated2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "Created", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ShoppingCarts", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShoppingCarts", "Created", c => c.DateTime());
            AlterColumn("dbo.Orders", "Created", c => c.DateTime());
        }
    }
}
