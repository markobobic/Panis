namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initilize : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClientSectors",
                c => new
                    {
                        ClientSectorID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SectorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ClientSectorID)
                .ForeignKey("dbo.Sectors", t => t.SectorID, cascadeDelete: true)
                .Index(t => t.SectorID);
            
            CreateTable(
                "dbo.Sectors",
                c => new
                    {
                        SectorID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        ClientSectorID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SectorID);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        EmployeeID = c.Int(),
                    })
                .PrimaryKey(t => t.DepartmentID);
            
            CreateTable(
                "dbo.Employees",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        LastName = c.String(nullable: false, maxLength: 50),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        Education = c.String(),
                        Mobile = c.String(),
                        ReportsTo = c.String(),
                        PhotoType = c.String(),
                        Photo = c.Binary(),
                        LivingCity = c.String(),
                        LivingStreet = c.String(),
                        LivingStreetNumber = c.String(),
                        CityFromID = c.String(),
                        StreetFromID = c.String(),
                        StreetNumberFromID = c.String(),
                        AppartmentNumberFromID = c.String(),
                        TeamLeadID = c.Int(),
                        SectorID = c.Int(),
                        ClientSectorID = c.Int(),
                        DepartmentID = c.Int(),
                        EmployeeEnrollmentID = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.ClientSectors", t => t.ClientSectorID)
                .ForeignKey("dbo.EmployeeEnrollments", t => t.EmployeeEnrollmentID)
                .ForeignKey("dbo.Sectors", t => t.SectorID)
                .ForeignKey("dbo.TeamLeads", t => t.TeamLeadID)
                .ForeignKey("dbo.Departments", t => t.DepartmentID)
                .Index(t => t.LastName)
                .Index(t => t.FirstName)
                .Index(t => t.TeamLeadID)
                .Index(t => t.SectorID)
                .Index(t => t.ClientSectorID)
                .Index(t => t.DepartmentID)
                .Index(t => t.EmployeeEnrollmentID);
            
            CreateTable(
                "dbo.EmployeeEnrollments",
                c => new
                    {
                        EmployeeEnrollmentID = c.Int(nullable: false, identity: true),
                        OfficialWorkStart = c.DateTime(),
                        WorkStart = c.DateTime(nullable: false),
                        Seniority = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeEnrollmentID)
                .Index(t => t.OfficialWorkStart);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ProjectID);
            
            CreateTable(
                "dbo.TeamLeads",
                c => new
                    {
                        TeamLeadID = c.Int(nullable: false, identity: true),
                        EmployeeID = c.Int(),
                    })
                .PrimaryKey(t => t.TeamLeadID);
            
            CreateTable(
                "dbo.Realizations",
                c => new
                    {
                        RealizationID = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Description = c.String(),
                        Start = c.DateTime(nullable: false),
                        RealizationTypeID = c.Int(),
                        ProjectID = c.Int(),
                        DepartmentID = c.Int(),
                        EmployeeID = c.Int(nullable: false),
                        Hours = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RealizationID)
                .ForeignKey("dbo.Departments", t => t.DepartmentID)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .ForeignKey("dbo.Projects", t => t.ProjectID)
                .ForeignKey("dbo.RealizationTypes", t => t.RealizationTypeID)
                .Index(t => t.RealizationTypeID)
                .Index(t => t.ProjectID)
                .Index(t => t.DepartmentID)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.RealizationTypes",
                c => new
                    {
                        RealizationTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.RealizationTypeID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ProjectEmployees",
                c => new
                    {
                        ProjectID = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectID, t.EmployeeID })
                .ForeignKey("dbo.Projects", t => t.ProjectID, cascadeDelete: true)
                .ForeignKey("dbo.Employees", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.ProjectID)
                .Index(t => t.EmployeeID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Realizations", "RealizationTypeID", "dbo.RealizationTypes");
            DropForeignKey("dbo.Realizations", "ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Realizations", "EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.Realizations", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.Employees", "DepartmentID", "dbo.Departments");
            DropForeignKey("dbo.Employees", "TeamLeadID", "dbo.TeamLeads");
            DropForeignKey("dbo.Employees", "SectorID", "dbo.Sectors");
            DropForeignKey("dbo.ProjectEmployees", "Employee_EmployeeID", "dbo.Employees");
            DropForeignKey("dbo.ProjectEmployees", "Project_ProjectID", "dbo.Projects");
            DropForeignKey("dbo.Employees", "EmployeeEnrollmentID", "dbo.EmployeeEnrollments");
            DropForeignKey("dbo.Employees", "ClientSectorID", "dbo.ClientSectors");
            DropForeignKey("dbo.ClientSectors", "SectorID", "dbo.Sectors");
            DropIndex("dbo.ProjectEmployees", new[] { "Employee_EmployeeID" });
            DropIndex("dbo.ProjectEmployees", new[] { "Project_ProjectID" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Realizations", new[] { "EmployeeID" });
            DropIndex("dbo.Realizations", new[] { "DepartmentID" });
            DropIndex("dbo.Realizations", new[] { "ProjectID" });
            DropIndex("dbo.Realizations", new[] { "RealizationTypeID" });
            DropIndex("dbo.EmployeeEnrollments", new[] { "OfficialWorkStart" });
            DropIndex("dbo.Employees", new[] { "EmployeeEnrollmentID" });
            DropIndex("dbo.Employees", new[] { "DepartmentID" });
            DropIndex("dbo.Employees", new[] { "ClientSectorID" });
            DropIndex("dbo.Employees", new[] { "SectorID" });
            DropIndex("dbo.Employees", new[] { "TeamLeadID" });
            DropIndex("dbo.Employees", new[] { "FirstName" });
            DropIndex("dbo.Employees", new[] { "LastName" });
            DropIndex("dbo.ClientSectors", new[] { "SectorID" });
            DropTable("dbo.ProjectEmployees");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RealizationTypes");
            DropTable("dbo.Realizations");
            DropTable("dbo.TeamLeads");
            DropTable("dbo.Projects");
            DropTable("dbo.EmployeeEnrollments");
            DropTable("dbo.Employees");
            DropTable("dbo.Departments");
            DropTable("dbo.Sectors");
            DropTable("dbo.ClientSectors");
        }
    }
}
