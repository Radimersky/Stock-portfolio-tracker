namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedNullableForeignkey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Product", "ProductCurrencyId", "dbo.Currency");
            DropIndex("dbo.Product", new[] { "ProductCurrencyId" });
            AlterColumn("dbo.Product", "ProductCurrencyId", c => c.Int(nullable: false));
            CreateIndex("dbo.Product", "ProductCurrencyId");
            AddForeignKey("dbo.Product", "ProductCurrencyId", "dbo.Currency", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Product", "ProductCurrencyId", "dbo.Currency");
            DropIndex("dbo.Product", new[] { "ProductCurrencyId" });
            AlterColumn("dbo.Product", "ProductCurrencyId", c => c.Int());
            CreateIndex("dbo.Product", "ProductCurrencyId");
            AddForeignKey("dbo.Product", "ProductCurrencyId", "dbo.Currency", "Id");
        }
    }
}
