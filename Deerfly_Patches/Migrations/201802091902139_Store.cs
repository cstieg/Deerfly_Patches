namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Store : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BaseUrl = c.String(maxLength: 100),
                        Name = c.String(maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        Country = c.String(maxLength: 2),
                        Currency = c.String(maxLength: 3),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Products", "Brand", c => c.String(maxLength: 70));
            AddColumn("dbo.Products", "Condition", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Products", "Condition");
            DropColumn("dbo.Products", "Brand");
            DropTable("dbo.Stores");
        }
    }
}
