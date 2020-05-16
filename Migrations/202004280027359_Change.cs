namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Employees", "EmployeeEnrollmentID", "dbo.EmployeeEnrollments");
            DropIndex("dbo.Employees", new[] { "EmployeeEnrollmentID" });
            AddColumn("dbo.EmployeeEnrollments", "EmployeeID", c => c.Int(nullable: false));
            CreateIndex("dbo.EmployeeEnrollments", "EmployeeID");
            AddForeignKey("dbo.EmployeeEnrollments", "EmployeeID", "dbo.Employees", "EmployeeID", cascadeDelete: true);
            DropColumn("dbo.Employees", "EmployeeEnrollmentID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Employees", "EmployeeEnrollmentID", c => c.Int());
            DropForeignKey("dbo.EmployeeEnrollments", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeeEnrollments", new[] { "EmployeeID" });
            DropColumn("dbo.EmployeeEnrollments", "EmployeeID");
            CreateIndex("dbo.Employees", "EmployeeEnrollmentID");
            AddForeignKey("dbo.Employees", "EmployeeEnrollmentID", "dbo.EmployeeEnrollments", "EmployeeEnrollmentID");
        }
    }
}
