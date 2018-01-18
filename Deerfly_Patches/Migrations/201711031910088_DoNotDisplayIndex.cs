namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DoNotDisplayIndex : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Addresses", "Phone");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Addresses", new[] { "Phone" });
        }
    }
}
