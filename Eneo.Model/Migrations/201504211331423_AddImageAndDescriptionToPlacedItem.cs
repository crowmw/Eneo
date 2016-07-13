namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddImageAndDescriptionToPlacedItem : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PlacedItems", "Description", c => c.String());
            AddColumn("dbo.PlacedItems", "ImageGuid", c => c.Guid(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.PlacedItems", "ImageGuid");
            DropColumn("dbo.PlacedItems", "Description");
        }
    }
}