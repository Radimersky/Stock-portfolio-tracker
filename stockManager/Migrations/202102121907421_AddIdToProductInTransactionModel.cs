namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdToProductInTransactionModel : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Transaction", name: "Product_Id", newName: "ProductId");
            RenameIndex(table: "dbo.Transaction", name: "IX_Product_Id", newName: "IX_ProductId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Transaction", name: "IX_ProductId", newName: "IX_Product_Id");
            RenameColumn(table: "dbo.Transaction", name: "ProductId", newName: "Product_Id");
        }
    }
}
