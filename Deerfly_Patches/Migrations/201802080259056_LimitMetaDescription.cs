namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LimitMetaDescription : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "MetaDescription", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Products", "MetaDescription", c => c.String());
        }
    }
}
