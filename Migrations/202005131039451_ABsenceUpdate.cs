namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ABsenceUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Absences", "EmployeeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Absences", "EmployeeID");
            AddForeignKey("dbo.Absences", "EmployeeID", "dbo.Employees", "EmployeeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Absences", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.Absences", new[] { "EmployeeID" });
            DropColumn("dbo.Absences", "EmployeeID");
        }
    }
}
