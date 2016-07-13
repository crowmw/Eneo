namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CommentTableIssue : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PlacedItemComments", name: "EneoUser_EneoUserID", newName: "EneoUserID");
            RenameIndex(table: "dbo.PlacedItemComments", name: "IX_EneoUser_EneoUserID", newName: "IX_EneoUserID");
        }

        public override void Down()
        {
            RenameIndex(table: "dbo.PlacedItemComments", name: "IX_EneoUserID", newName: "IX_EneoUser_EneoUserID");
            RenameColumn(table: "dbo.PlacedItemComments", name: "EneoUserID", newName: "EneoUser_EneoUserID");
        }
    }
}