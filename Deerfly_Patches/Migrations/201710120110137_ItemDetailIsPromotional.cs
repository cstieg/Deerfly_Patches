namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ItemDetailIsPromotional : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "IsPromotionalItem", c => c.Boolean(nullable: false));
            DropColumn("dbo.OrderDetails", "CheckedOut");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OrderDetails", "CheckedOut", c => c.Boolean(nullable: false));
            DropColumn("dbo.OrderDetails", "IsPromotionalItem");
        }
    }
}
