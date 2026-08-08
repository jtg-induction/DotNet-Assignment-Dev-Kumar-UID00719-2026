namespace dotNetAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuditsInJwtTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RefreshTokens", "CreatedAt", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RefreshTokens", "CreatedAt");
        }
    }
}
