namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeesProject : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeesProjects",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(),
                        ProjectID = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Employees", t => t.EmployeeID)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .Index(t => t.EmployeeID)
                .Index(t => t.ProjectID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EmployeesProjects", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.EmployeesProjects", "EmployeeID", "dbo.Employees");
            DropIndex("dbo.EmployeesProjects", new[] { "ProjectID" });
            DropIndex("dbo.EmployeesProjects", new[] { "EmployeeID" });
            DropTable("dbo.EmployeesProjects");
        }
    }
}
