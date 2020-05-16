namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPositions : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "EmpPosition", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "EmpPosition");
        }
    }
}
