namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class NotificationAndMessage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        MessageID = c.Int(nullable: false, identity: true),
                        SenderUserID = c.String(),
                        ReceiverUserID = c.String(),
                        Content = c.String(),
                        SendDate = c.DateTime(nullable: false),
                        Readed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MessageID);

            CreateTable(
                "dbo.Notifications",
                c => new
                    {
                        NotificationID = c.Int(nullable: false, identity: true),
                        EneoUserID = c.String(maxLength: 128),
                        Content = c.String(),
                        NotificationType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationID)
                .ForeignKey("dbo.EneoUsers", t => t.EneoUserID)
                .Index(t => t.EneoUserID);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Notifications", "EneoUserID", "dbo.EneoUsers");
            DropIndex("dbo.Notifications", new[] { "EneoUserID" });
            DropTable("dbo.Notifications");
            DropTable("dbo.Messages");
        }
    }
}