namespace dotNetAssignment.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUniquePhoneNumberToUser : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Users", "PhoneNumber", c => c.String(nullable: false, maxLength: 10));
            CreateIndex("dbo.Users", "PhoneNumber", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "PhoneNumber" });
            AlterColumn("dbo.Users", "PhoneNumber", c => c.String(nullable: false));
        }
    }
}
