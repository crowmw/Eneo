namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddDateToCommentAndPlacedItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlacedItems", "AddedDate", c => c.DateTime());
        }

        public override void Down()
        {
            DropColumn("dbo.PlacedItems", "AddedDate");
        }
    }
}