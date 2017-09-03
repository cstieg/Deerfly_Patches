namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testimonialImageUrlNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Testimonials", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Testimonials", "ImageUrl", c => c.String(nullable: false));
        }
    }
}
