namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testimonials11 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Testimonials", "DateAdded");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Testimonials", "DateAdded", c => c.DateTime(nullable: false));
        }
    }
}
