namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Chat : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UsersChats",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        FromUserId = c.String(),
                        OtherUserId = c.String(),
                        RoomId = c.String(),
                        ClientGuidId = c.String(),
                        IsSeen = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.AspNetUsers", "ImagePath", c => c.String());
            AddColumn("dbo.AspNetUsers", "LastActivity", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "LastActivity");
            DropColumn("dbo.AspNetUsers", "ImagePath");
            DropTable("dbo.UsersChats");
        }
    }
}
