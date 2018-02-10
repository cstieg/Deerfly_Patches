namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cartIndex : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "Cart", c => c.String(maxLength: 20));
            AlterColumn("dbo.ShoppingCarts", "OwnerId", c => c.String(maxLength: 36));
            CreateIndex("dbo.Orders", "Cart");
            CreateIndex("dbo.ShoppingCarts", "OwnerId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.ShoppingCarts", new[] { "OwnerId" });
            DropIndex("dbo.Orders", new[] { "Cart" });
            AlterColumn("dbo.ShoppingCarts", "OwnerId", c => c.String());
            AlterColumn("dbo.Orders", "Cart", c => c.String());
        }
    }
}
