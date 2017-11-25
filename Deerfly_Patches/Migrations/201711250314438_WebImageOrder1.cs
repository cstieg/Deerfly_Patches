namespace Deerfly_Patches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WebImageOrder1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WebImages", "Order", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.WebImages", "Order");
        }
    }
}
