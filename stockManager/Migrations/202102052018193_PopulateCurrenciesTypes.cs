namespace stockManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateCurrenciesTypes : DbMigration
    {
        public override void Up()
        {
            Sql("INSERT INTO Currency (Name) VALUES ('EUR')");
            Sql("INSERT INTO Currency (Name) VALUES ('USD')");
            Sql("INSERT INTO Currency (Name) VALUES ('CZK')");
        }
        
        public override void Down()
        {
        }
    }
}
