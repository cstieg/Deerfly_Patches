namespace DeerflyPatches.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeaddressrequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Addresses", "Address1", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "City", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "State", c => c.String(maxLength: 50));
            AlterColumn("dbo.Addresses", "Zip", c => c.String(maxLength: 15));
            AlterColumn("dbo.Addresses", "Country", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "Country", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Addresses", "Zip", c => c.String(nullable: false, maxLength: 15));
            AlterColumn("dbo.Addresses", "State", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Addresses", "City", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Addresses", "Address1", c => c.String(nullable: false, maxLength: 50));
        }
    }
}
