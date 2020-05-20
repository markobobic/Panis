namespace Panis.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectorProject : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "SectorID", c => c.Int());
            CreateIndex("dbo.Projects", "SectorID");
            AddForeignKey("dbo.Projects", "SectorID", "dbo.Sectors", "SectorID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "SectorID", "dbo.Sectors");
            DropIndex("dbo.Projects", new[] { "SectorID" });
            DropColumn("dbo.Projects", "SectorID");
        }
    }
}
