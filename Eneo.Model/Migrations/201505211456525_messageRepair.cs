namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class messageRepair : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Messages", "SenderUserID", c => c.String(maxLength: 128));
            AlterColumn("dbo.Messages", "ReceiverUserID", c => c.String(maxLength: 128));
            CreateIndex("dbo.Messages", "SenderUserID");
            CreateIndex("dbo.Messages", "ReceiverUserID");
            AddForeignKey("dbo.Messages", "ReceiverUserID", "dbo.EneoUsers", "EneoUserID");
            AddForeignKey("dbo.Messages", "SenderUserID", "dbo.EneoUsers", "EneoUserID");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Messages", "SenderUserID", "dbo.EneoUsers");
            DropForeignKey("dbo.Messages", "ReceiverUserID", "dbo.EneoUsers");
            DropIndex("dbo.Messages", new[] { "ReceiverUserID" });
            DropIndex("dbo.Messages", new[] { "SenderUserID" });
            AlterColumn("dbo.Messages", "ReceiverUserID", c => c.String());
            AlterColumn("dbo.Messages", "SenderUserID", c => c.String());
        }
    }
}