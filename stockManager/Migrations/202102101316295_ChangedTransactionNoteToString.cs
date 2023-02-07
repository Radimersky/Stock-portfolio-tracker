namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedTransactionNoteToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transaction", "Note", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transaction", "Note", c => c.Boolean(nullable: false));
        }
    }
}
