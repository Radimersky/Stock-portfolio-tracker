namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedIdOfTransactionToInt : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Transaction", new[] { "TotalValueCurrency_Id" });
            DropIndex("dbo.Transaction", new[] { "TransactionCostsCurrency_Id" });
            DropColumn("dbo.Transaction", "TotalValueCurrencyId");
            DropColumn("dbo.Transaction", "TransactionCostsCurrencyId");
            RenameColumn(table: "dbo.Transaction", name: "TotalValueCurrency_Id", newName: "TotalValueCurrencyId");
            RenameColumn(table: "dbo.Transaction", name: "TransactionCostsCurrency_Id", newName: "TransactionCostsCurrencyId");
            AlterColumn("dbo.Transaction", "TransactionCostsCurrencyId", c => c.Int());
            AlterColumn("dbo.Transaction", "TotalValueCurrencyId", c => c.Int());
            CreateIndex("dbo.Transaction", "TransactionCostsCurrencyId");
            CreateIndex("dbo.Transaction", "TotalValueCurrencyId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Transaction", new[] { "TotalValueCurrencyId" });
            DropIndex("dbo.Transaction", new[] { "TransactionCostsCurrencyId" });
            AlterColumn("dbo.Transaction", "TotalValueCurrencyId", c => c.Byte(nullable: false));
            AlterColumn("dbo.Transaction", "TransactionCostsCurrencyId", c => c.Byte(nullable: false));
            RenameColumn(table: "dbo.Transaction", name: "TransactionCostsCurrencyId", newName: "TransactionCostsCurrency_Id");
            RenameColumn(table: "dbo.Transaction", name: "TotalValueCurrencyId", newName: "TotalValueCurrency_Id");
            AddColumn("dbo.Transaction", "TransactionCostsCurrencyId", c => c.Byte(nullable: false));
            AddColumn("dbo.Transaction", "TotalValueCurrencyId", c => c.Byte(nullable: false));
            CreateIndex("dbo.Transaction", "TransactionCostsCurrency_Id");
            CreateIndex("dbo.Transaction", "TotalValueCurrency_Id");
        }
    }
}
