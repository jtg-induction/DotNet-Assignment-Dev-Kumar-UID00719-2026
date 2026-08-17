namespace dotNetAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuantityAvailableAndChangeRestaurantRatingToDecimal : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Menus", "QuantityAvailable", c => c.Int(nullable: false));
            AlterColumn("dbo.Restaurants", "Rating", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Restaurants", "Rating", c => c.Int(nullable: false));
            DropColumn("dbo.Menus", "QuantityAvailable");
        }
    }
}
