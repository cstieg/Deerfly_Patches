namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testimonialAllowRelativeUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Testimonials", "ImageSrcSet", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Testimonials", "ImageSrcSet");
        }
    }
}
