namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Absences : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Absences",
                c => new
                    {
                        AbsenceID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Start = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                        AbsenceTypeID = c.Int(),
                        AbsenceOfEmployeeID = c.Int(),
                    })
                .PrimaryKey(t => t.AbsenceID)
                .ForeignKey("dbo.AbsenceOfEmployees", t => t.AbsenceOfEmployeeID)
                .ForeignKey("dbo.AbsenceTypes", t => t.AbsenceTypeID)
                .Index(t => t.AbsenceTypeID)
                .Index(t => t.AbsenceOfEmployeeID);
            
            CreateTable(
                "dbo.AbsenceOfEmployees",
                c => new
                    {
                        AbsenceLimitID = c.Int(nullable: false, identity: true),
                        Year = c.DateTime(nullable: false),
                        NumberOfVacations = c.Int(nullable: false),
                        NumberOfSickLeave = c.Int(nullable: false),
                        NumberOfFreeDays = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AbsenceLimitID);
            
            CreateTable(
                "dbo.AbsenceTypes",
                c => new
                    {
                        AbsenceTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.AbsenceTypeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Absences", "AbsenceTypeID", "dbo.AbsenceTypes");
            DropForeignKey("dbo.Absences", "AbsenceOfEmployeeID", "dbo.AbsenceOfEmployees");
            DropIndex("dbo.Absences", new[] { "AbsenceOfEmployeeID" });
            DropIndex("dbo.Absences", new[] { "AbsenceTypeID" });
            DropTable("dbo.AbsenceTypes");
            DropTable("dbo.AbsenceOfEmployees");
            DropTable("dbo.Absences");
        }
    }
}
