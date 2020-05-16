namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NotifyKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Notifications", "EmployeeID", c => c.Int());
            CreateIndex("dbo.Notifications", "EmployeeID");
            AddForeignKey("dbo.Notifications", "EmployeeID", "dbo.Employees", "EmployeeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.Notifications", new[] { "EmployeeID" });
            DropColumn("dbo.Notifications", "EmployeeID");
        }
    }
}
