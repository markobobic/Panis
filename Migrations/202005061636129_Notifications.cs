namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Notifications : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "CountNotifications", c => c.Int());
            AddColumn("dbo.AspNetUsers", "ReadNotifications", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ReadNotifications");
            DropColumn("dbo.AspNetUsers", "CountNotifications");
        }
    }
}
