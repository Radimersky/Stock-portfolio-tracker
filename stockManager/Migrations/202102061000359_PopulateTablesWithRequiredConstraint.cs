namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateTablesWithRequiredConstraint : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Currency", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Market", "MIC", c => c.String(nullable: false));
            AlterColumn("dbo.Product", "ISIN", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Product", "ISIN", c => c.String());
            AlterColumn("dbo.Market", "MIC", c => c.String());
            AlterColumn("dbo.Currency", "Name", c => c.String());
        }
    }
}
