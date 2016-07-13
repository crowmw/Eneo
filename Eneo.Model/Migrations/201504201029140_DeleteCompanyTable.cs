namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class DeleteCompanyTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Companies", "UserManagerID", "dbo.EneoUsers");
            DropIndex("dbo.Companies", new[] { "UserManagerID" });
            DropTable("dbo.Companies");
        }

        public override void Down()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(),
                        NIP = c.String(),
                        UserManagerID = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CompanyID);

            CreateIndex("dbo.Companies", "UserManagerID");
            AddForeignKey("dbo.Companies", "UserManagerID", "dbo.EneoUsers", "EneoUserID");
        }
    }
}