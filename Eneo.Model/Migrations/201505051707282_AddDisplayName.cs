namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddDisplayName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EneoUsers", "DisplayName", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.EneoUsers", "DisplayName");
        }
    }
}