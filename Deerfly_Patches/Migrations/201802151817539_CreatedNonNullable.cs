namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatedNonNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ShoppingCarts", "Created", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ShoppingCarts", "Created", c => c.DateTime());
        }
    }
}
