namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PromoCodeNullableDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PromoCodes", "CodeStart", c => c.DateTime());
            AlterColumn("dbo.PromoCodes", "CodeEnd", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PromoCodes", "CodeEnd", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PromoCodes", "CodeStart", c => c.DateTime(nullable: false));
        }
    }
}
