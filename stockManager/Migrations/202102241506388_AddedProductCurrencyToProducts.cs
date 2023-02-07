namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProductCurrencyToProducts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Product", "ProductCurrency_Id", c => c.Int());
            CreateIndex("dbo.Product", "ProductCurrency_Id");
            AddForeignKey("dbo.Product", "ProductCurrency_Id", "dbo.Currency", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "ProductCurrency_Id", "dbo.Currency");
            DropIndex("dbo.Product", new[] { "ProductCurrency_Id" });
            DropColumn("dbo.Product", "ProductCurrency_Id");
        }
    }
}
