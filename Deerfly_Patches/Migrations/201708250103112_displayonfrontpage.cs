namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class displayonfrontpage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "DisplayOnFrontPage", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "DisplayOnFrontPage");
        }
    }
}
