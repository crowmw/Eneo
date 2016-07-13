namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DateToCollectionAndCollectedItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CollectionItems", "CompanyID", "dbo.Companies");
            DropIndex("dbo.CollectionItems", new[] { "CompanyID" });
            AddColumn("dbo.CollectedItems", "CatchDate", c => c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()"));
            AddColumn("dbo.CollectionItems", "AddedDate", c => c.DateTime(nullable: false, defaultValueSql: "GETUTCDATE()"));
            DropColumn("dbo.CollectionItems", "CompanyID");
        }

        public override void Down()
        {
            AddColumn("dbo.CollectionItems", "CompanyID", c => c.Int(nullable: false));
            DropColumn("dbo.CollectionItems", "AddedDate");
            DropColumn("dbo.CollectedItems", "CatchDate");
            CreateIndex("dbo.CollectionItems", "CompanyID");
            AddForeignKey("dbo.CollectionItems", "CompanyID", "dbo.Companies", "CompanyID", cascadeDelete: true);
        }
    }
}