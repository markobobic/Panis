namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DataTypeChange : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Absences", "AbsenceOfEmployeeID", "dbo.AbsenceOfEmployees");
            DropForeignKey("dbo.Absences", "AbsenceTypeID", "dbo.AbsenceTypes");
            DropForeignKey("dbo.Realizations", "RealizationTypeID", "dbo.RealizationTypes");
            DropIndex("dbo.Absences", new[] { "AbsenceTypeID" });
            DropIndex("dbo.Absences", new[] { "AbsenceOfEmployeeID" });
            DropIndex("dbo.Realizations", new[] { "RealizationTypeID" });
            DropPrimaryKey("dbo.AbsenceTypes");
            DropPrimaryKey("dbo.RealizationTypes");
            AlterColumn("dbo.Absences", "AbsenceTypeID", c => c.Byte());
            AlterColumn("dbo.AbsenceTypes", "AbsenceTypeID", c => c.Byte(nullable: false));
            AlterColumn("dbo.Realizations", "RealizationTypeID", c => c.Byte());
            AlterColumn("dbo.Realizations", "Hours", c => c.Short(nullable: false));
            AlterColumn("dbo.RealizationTypes", "RealizationTypeID", c => c.Byte(nullable: false));
            AddPrimaryKey("dbo.AbsenceTypes", "AbsenceTypeID");
            AddPrimaryKey("dbo.RealizationTypes", "RealizationTypeID");
            CreateIndex("dbo.Absences", "AbsenceTypeID");
            CreateIndex("dbo.Realizations", "RealizationTypeID");
            AddForeignKey("dbo.Absences", "AbsenceTypeID", "dbo.AbsenceTypes", "AbsenceTypeID");
            AddForeignKey("dbo.Realizations", "RealizationTypeID", "dbo.RealizationTypes", "RealizationTypeID");
            DropColumn("dbo.Absences", "AbsenceOfEmployeeID");
            DropTable("dbo.AbsenceOfEmployees");
        }
        
        public override void Down()
        {
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
            
            AddColumn("dbo.Absences", "AbsenceOfEmployeeID", c => c.Int());
            DropForeignKey("dbo.Realizations", "RealizationTypeID", "dbo.RealizationTypes");
            DropForeignKey("dbo.Absences", "AbsenceTypeID", "dbo.AbsenceTypes");
            DropIndex("dbo.Realizations", new[] { "RealizationTypeID" });
            DropIndex("dbo.Absences", new[] { "AbsenceTypeID" });
            DropPrimaryKey("dbo.RealizationTypes");
            DropPrimaryKey("dbo.AbsenceTypes");
            AlterColumn("dbo.RealizationTypes", "RealizationTypeID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Realizations", "Hours", c => c.Int(nullable: false));
            AlterColumn("dbo.Realizations", "RealizationTypeID", c => c.Int());
            AlterColumn("dbo.AbsenceTypes", "AbsenceTypeID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.Absences", "AbsenceTypeID", c => c.Int());
            AddPrimaryKey("dbo.RealizationTypes", "RealizationTypeID");
            AddPrimaryKey("dbo.AbsenceTypes", "AbsenceTypeID");
            CreateIndex("dbo.Realizations", "RealizationTypeID");
            CreateIndex("dbo.Absences", "AbsenceOfEmployeeID");
            CreateIndex("dbo.Absences", "AbsenceTypeID");
            AddForeignKey("dbo.Realizations", "RealizationTypeID", "dbo.RealizationTypes", "RealizationTypeID");
            AddForeignKey("dbo.Absences", "AbsenceTypeID", "dbo.AbsenceTypes", "AbsenceTypeID");
            AddForeignKey("dbo.Absences", "AbsenceOfEmployeeID", "dbo.AbsenceOfEmployees", "AbsenceLimitID");
        }
    }
}
