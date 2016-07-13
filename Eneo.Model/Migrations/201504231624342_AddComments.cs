namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddComments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlacedItemComments",
                c => new
                    {
                        CommentID = c.Int(nullable: false, identity: true),
                        PlacedItemID = c.Int(nullable: false),
                        Content = c.String(nullable: false),
                        UserID = c.String(nullable: false),
                        EneoUser_EneoUserID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CommentID)
                .ForeignKey("dbo.EneoUsers", t => t.EneoUser_EneoUserID)
                .ForeignKey("dbo.PlacedItems", t => t.PlacedItemID, cascadeDelete: true)
                .Index(t => t.PlacedItemID)
                .Index(t => t.EneoUser_EneoUserID);

            AddColumn("dbo.PlacedItems", "Stars", c => c.Double());
        }

        public override void Down()
        {
            DropForeignKey("dbo.PlacedItemComments", "PlacedItemID", "dbo.PlacedItems");
            DropForeignKey("dbo.PlacedItemComments", "EneoUser_EneoUserID", "dbo.EneoUsers");
            DropIndex("dbo.PlacedItemComments", new[] { "EneoUser_EneoUserID" });
            DropIndex("dbo.PlacedItemComments", new[] { "PlacedItemID" });
            DropColumn("dbo.PlacedItems", "Stars");
            DropTable("dbo.PlacedItemComments");
        }
    }
}