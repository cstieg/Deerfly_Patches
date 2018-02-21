namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShoppingCartOrderCreateDate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders");
            DropIndex("dbo.ShoppingCarts", new[] { "OrderId" });
            AddColumn("dbo.Orders", "Created", c => c.DateTime());
            AddColumn("dbo.ShoppingCarts", "Created", c => c.DateTime());
            AlterColumn("dbo.ShoppingCarts", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.ShoppingCarts", "OrderId");
            AddForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders");
            DropIndex("dbo.ShoppingCarts", new[] { "OrderId" });
            AlterColumn("dbo.ShoppingCarts", "OrderId", c => c.Int());
            DropColumn("dbo.ShoppingCarts", "Created");
            DropColumn("dbo.Orders", "Created");
            CreateIndex("dbo.ShoppingCarts", "OrderId");
            AddForeignKey("dbo.ShoppingCarts", "OrderId", "dbo.Orders", "Id");
        }
    }
}
