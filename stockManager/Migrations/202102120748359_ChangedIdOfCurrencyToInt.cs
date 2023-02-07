namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedIdOfCurrencyToInt : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Transaction", "StockPriceCurrency_Id", "dbo.Currency");
            DropIndex("dbo.Transaction", new[] { "StockPriceCurrency_Id" });
            DropColumn("dbo.Transaction", "StockPriceCurrencyId");
            RenameColumn(table: "dbo.Transaction", name: "StockPriceCurrency_Id", newName: "StockPriceCurrencyId");
            AlterColumn("dbo.Transaction", "StockPriceCurrencyId", c => c.Int(nullable: false));
            AlterColumn("dbo.Transaction", "StockPriceCurrencyId", c => c.Int(nullable: false));
            CreateIndex("dbo.Transaction", "StockPriceCurrencyId");
            AddForeignKey("dbo.Transaction", "StockPriceCurrencyId", "dbo.Currency", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transaction", "StockPriceCurrencyId", "dbo.Currency");
            DropIndex("dbo.Transaction", new[] { "StockPriceCurrencyId" });
            AlterColumn("dbo.Transaction", "StockPriceCurrencyId", c => c.Int());
            AlterColumn("dbo.Transaction", "StockPriceCurrencyId", c => c.Byte(nullable: false));
            RenameColumn(table: "dbo.Transaction", name: "StockPriceCurrencyId", newName: "StockPriceCurrency_Id");
            AddColumn("dbo.Transaction", "StockPriceCurrencyId", c => c.Byte(nullable: false));
            CreateIndex("dbo.Transaction", "StockPriceCurrency_Id");
            AddForeignKey("dbo.Transaction", "StockPriceCurrency_Id", "dbo.Currency", "Id");
        }
    }
}
