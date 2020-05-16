namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApplicationDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Absences", "ApplicationDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Absences", "ApplicationDate");
        }
    }
}
