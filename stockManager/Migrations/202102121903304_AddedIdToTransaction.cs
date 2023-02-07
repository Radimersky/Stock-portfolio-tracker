namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedIdToTransaction : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Transaction", name: "Market_Id", newName: "MarketId");
            RenameIndex(table: "dbo.Transaction", name: "IX_Market_Id", newName: "IX_MarketId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Transaction", name: "IX_MarketId", newName: "IX_Market_Id");
            RenameColumn(table: "dbo.Transaction", name: "MarketId", newName: "Market_Id");
        }
    }
}
