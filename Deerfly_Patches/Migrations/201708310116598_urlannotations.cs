namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class urlannotations : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "ImageURL", c => c.String());
            AlterColumn("dbo.Testimonials", "ImageUrl", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Testimonials", "ImageUrl", c => c.String(nullable: false, maxLength: 255));
            AlterColumn("dbo.Products", "ImageURL", c => c.String(maxLength: 255));
        }
    }
}
