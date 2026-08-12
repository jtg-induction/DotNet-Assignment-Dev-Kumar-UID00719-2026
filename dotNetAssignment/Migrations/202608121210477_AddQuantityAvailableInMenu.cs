namespace dotNetAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuantityAvailableInMenu : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "QuantityAvailable", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Menus", "QuantityAvailable");
        }
    }
}
