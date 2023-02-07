namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNoteToTransaction : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transaction", "Note", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Transaction", "Note");
        }
    }
}
