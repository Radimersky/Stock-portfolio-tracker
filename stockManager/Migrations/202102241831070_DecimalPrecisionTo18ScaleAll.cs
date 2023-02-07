namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecimalPrecisionTo18ScaleAll : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transaction", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 8));
            AlterColumn("dbo.Transaction", "StockPrice", c => c.Decimal(nullable: false, precision: 18, scale: 8));
            AlterColumn("dbo.Transaction", "ExchangeRate", c => c.Decimal(nullable: false, precision: 18, scale: 8));
            AlterColumn("dbo.Transaction", "TransactionCosts", c => c.Decimal(nullable: false, precision: 18, scale: 8));
            AlterColumn("dbo.Transaction", "TotalValue", c => c.Decimal(nullable: false, precision: 18, scale: 8));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transaction", "TotalValue", c => c.Decimal(nullable: false, precision: 10, scale: 8));
            AlterColumn("dbo.Transaction", "TransactionCosts", c => c.Decimal(nullable: false, precision: 10, scale: 8));
            AlterColumn("dbo.Transaction", "ExchangeRate", c => c.Decimal(nullable: false, precision: 10, scale: 8));
            AlterColumn("dbo.Transaction", "StockPrice", c => c.Decimal(nullable: false, precision: 10, scale: 8));
            AlterColumn("dbo.Transaction", "Amount", c => c.Decimal(nullable: false, precision: 10, scale: 8));
        }
    }
}
