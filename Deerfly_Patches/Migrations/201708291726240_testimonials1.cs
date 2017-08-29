namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testimonials1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Testimonials",
                c => new
                    {
                        TestimonialId = c.Int(nullable: false, identity: true),
                        Label = c.String(nullable: false, maxLength: 50),
                        Date = c.DateTime(),
                        DateAdded = c.DateTime(nullable: false),
                        ImageUrl = c.String(nullable: false, maxLength: 255),
                    })
                .PrimaryKey(t => t.TestimonialId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Testimonials");
        }
    }
}
