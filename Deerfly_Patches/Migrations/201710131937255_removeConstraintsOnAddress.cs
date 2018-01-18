namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeConstraintsOnAddress : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Orders", "Subtotal");
            DropColumn("dbo.Orders", "Shipping");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Orders", "Shipping", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Orders", "Subtotal", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
