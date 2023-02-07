namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedAttributeInProductTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "Ticker", c => c.String(nullable: false));
            AlterColumn("dbo.Product", "ISIN", c => c.String());
            DropColumn("dbo.Transaction", "Ticker");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Transaction", "Ticker", c => c.String());
            AlterColumn("dbo.Product", "ISIN", c => c.String(nullable: false));
            DropColumn("dbo.Product", "Ticker");
        }
    }
}
