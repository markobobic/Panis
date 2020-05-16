namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Approved : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Absences", "Approved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Absences", "Approved");
        }
    }
}
