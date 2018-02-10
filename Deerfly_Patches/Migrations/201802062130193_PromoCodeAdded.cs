namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoCodeAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PromoCodeAddeds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShoppingCartId = c.Int(nullable: false),
                        PromoCodeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PromoCodes", t => t.PromoCodeId, cascadeDelete: true)
                .ForeignKey("dbo.ShoppingCarts", t => t.ShoppingCartId, cascadeDelete: true)
                .Index(t => t.ShoppingCartId)
                .Index(t => t.PromoCodeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PromoCodeAddeds", "ShoppingCartId", "dbo.ShoppingCarts");
            DropForeignKey("dbo.PromoCodeAddeds", "PromoCodeId", "dbo.PromoCodes");
            DropIndex("dbo.PromoCodeAddeds", new[] { "PromoCodeId" });
            DropIndex("dbo.PromoCodeAddeds", new[] { "ShoppingCartId" });
            DropTable("dbo.PromoCodeAddeds");
        }
    }
}
