namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GTIN : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "GoogleProductCategory", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "GoogleProductCategory");
        }
    }
}
