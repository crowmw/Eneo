namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddExternalProvider : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EneoUsers", "UserIDFromProvider", c => c.String());
            AddColumn("dbo.EneoUsers", "Provider", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.EneoUsers", "Provider");
            DropColumn("dbo.EneoUsers", "UserIDFromProvider");
        }
    }
}