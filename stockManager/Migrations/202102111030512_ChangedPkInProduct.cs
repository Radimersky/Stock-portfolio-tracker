namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedPkInProduct : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Product", "ISIN", c => c.String(nullable: false));
            AlterColumn("dbo.Product", "Ticker", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "Ticker", c => c.String(nullable: false));
            AlterColumn("dbo.Product", "ISIN", c => c.String());
        }
    }
}
