namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class AddAvatarGuid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EneoUsers", "AvatarGuid", c => c.Guid());
        }

        public override void Down()
        {
            DropColumn("dbo.EneoUsers", "AvatarGuid");
        }
    }
}