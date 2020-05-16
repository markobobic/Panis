namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatingUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EmployeeID", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "IsActive", c => c.Boolean(nullable: false));
            CreateIndex("dbo.AspNetUsers", "EmployeeID");
            AddForeignKey("dbo.AspNetUsers", "EmployeeID", "dbo.Employees", "EmployeeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.AspNetUsers", new[] { "EmployeeID" });
            DropColumn("dbo.AspNetUsers", "IsActive");
            DropColumn("dbo.AspNetUsers", "EmployeeID");
        }
    }
}
