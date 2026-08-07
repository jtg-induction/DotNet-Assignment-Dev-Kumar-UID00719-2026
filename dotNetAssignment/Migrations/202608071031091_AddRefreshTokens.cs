namespace dotNetAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRefreshTokens : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        JwtId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.JwtId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RefreshTokens");
        }
    }
}
