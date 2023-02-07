namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DecimalPrecisionTo18Scale : DbMigration
    {
        public override void Up()
        {
  
            AlterColumn("dbo.Transaction", "TotalValue", c => c.Decimal(nullable: false, precision: 18, scale: 8));
        }
        
        public override void Down()
        {
          
            AlterColumn("dbo.Transaction", "Amount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
