namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DmpUserPositions : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DMPUserPositions",
                c => new
                    {
                        DMPUserPositionID = c.Int(nullable: false, identity: true),
                        EneoUserID = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Longitude = c.String(),
                        Latitude = c.String(),
                    })
                .PrimaryKey(t => t.DMPUserPositionID)
                .ForeignKey("dbo.EneoUsers", t => t.EneoUserID)
                .Index(t => t.EneoUserID);
        }

        public override void Down()
        {
            DropForeignKey("dbo.DMPUserPositions", "EneoUserID", "dbo.EneoUsers");
            DropIndex("dbo.DMPUserPositions", new[] { "EneoUserID" });
            DropTable("dbo.DMPUserPositions");
        }
    }
}