namespace DOGEOnlineGeneralEditor.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class droppedFilecount : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Project", "FileCount");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Project", "FileCount", c => c.Int(nullable: false));
        }
    }
}
