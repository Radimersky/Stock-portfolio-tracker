namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductCurrencyIdToProducts : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Product", name: "ProductCurrency_Id", newName: "ProductCurrencyId");
            RenameIndex(table: "dbo.Product", name: "IX_ProductCurrency_Id", newName: "IX_ProductCurrencyId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Product", name: "IX_ProductCurrencyId", newName: "IX_ProductCurrency_Id");
            RenameColumn(table: "dbo.Product", name: "ProductCurrencyId", newName: "ProductCurrency_Id");
        }
    }
}
