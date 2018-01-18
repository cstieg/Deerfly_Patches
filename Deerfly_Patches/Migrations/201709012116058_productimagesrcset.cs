namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productimagesrcset : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "ImageSrcSet", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "ImageSrcSet");
        }
    }
}
