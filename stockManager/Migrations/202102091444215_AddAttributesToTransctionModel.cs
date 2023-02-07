namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAttributesToTransctionModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Transaction", "Market_Id", c => c.Int(nullable: false));
            AddColumn("dbo.Transaction", "Product_Id", c => c.Int(nullable: false));
            CreateIndex("dbo.Transaction", "Market_Id");
            CreateIndex("dbo.Transaction", "Product_Id");
            AddForeignKey("dbo.Transaction", "Market_Id", "dbo.Market", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Transaction", "Product_Id", "dbo.Product", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transaction", "Product_Id", "dbo.Product");
            DropForeignKey("dbo.Transaction", "Market_Id", "dbo.Market");
            DropIndex("dbo.Transaction", new[] { "Product_Id" });
            DropIndex("dbo.Transaction", new[] { "Market_Id" });
            DropColumn("dbo.Transaction", "Product_Id");
            DropColumn("dbo.Transaction", "Market_Id");
        }
    }
}
