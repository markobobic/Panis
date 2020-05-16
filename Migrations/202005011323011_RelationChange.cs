namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RelationChange : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Departments", "EmployeeID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Departments", "EmployeeID", c => c.Int());
        }
    }
}
