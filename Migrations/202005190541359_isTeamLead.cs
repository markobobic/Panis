namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isTeamLead : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employees", "IsTeamLead", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employees", "IsTeamLead");
        }
    }
}
