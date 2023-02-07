namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTableTransactionAndCurrencies : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Currency",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Transaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DateTime = c.DateTime(nullable: false),
                        Ticker = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockPriceCurrencyId = c.Byte(nullable: false),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionCosts = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionCostsCurrencyId = c.Byte(nullable: false),
                        TotalValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalValueCurrencyId = c.Byte(nullable: false),
                        IsBuy = c.Boolean(nullable: false),
                        StockPriceCurrency_Id = c.Int(),
                        TotalValueCurrency_Id = c.Int(),
                        TransactionCostsCurrency_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Currency", t => t.StockPriceCurrency_Id)
                .ForeignKey("dbo.Currency", t => t.TotalValueCurrency_Id)
                .ForeignKey("dbo.Currency", t => t.TransactionCostsCurrency_Id)
                .Index(t => t.StockPriceCurrency_Id)
                .Index(t => t.TotalValueCurrency_Id)
                .Index(t => t.TransactionCostsCurrency_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transaction", "TransactionCostsCurrency_Id", "dbo.Currency");
            DropForeignKey("dbo.Transaction", "TotalValueCurrency_Id", "dbo.Currency");
            DropForeignKey("dbo.Transaction", "StockPriceCurrency_Id", "dbo.Currency");
            DropIndex("dbo.Transaction", new[] { "TransactionCostsCurrency_Id" });
            DropIndex("dbo.Transaction", new[] { "TotalValueCurrency_Id" });
            DropIndex("dbo.Transaction", new[] { "StockPriceCurrency_Id" });
            DropTable("dbo.Transaction");
            DropTable("dbo.Currency");
        }
    }
}
