namespace Eneo.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Secret = c.String(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        ApplicationType = c.Int(nullable: false),
                        Active = c.Boolean(nullable: false),
                        RefreshTokenLifeTime = c.Int(nullable: false),
                        AllowedOrigin = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.CollectedItems",
                c => new
                    {
                        CollectedItemID = c.Int(nullable: false, identity: true),
                        EneoUserID = c.String(nullable: false, maxLength: 128),
                        PlacedItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CollectedItemID)
                .ForeignKey("dbo.EneoUsers", t => t.EneoUserID, cascadeDelete: true)
                .ForeignKey("dbo.PlacedItems", t => t.PlacedItemID, cascadeDelete: true)
                .Index(t => t.EneoUserID)
                .Index(t => t.PlacedItemID);

            CreateTable(
                "dbo.EneoUsers",
                c => new
                    {
                        EneoUserID = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        LastLoginDate = c.DateTime(),
                        RegisterDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.EneoUserID);

            CreateTable(
                "dbo.PlacedItems",
                c => new
                    {
                        PlacedItemID = c.Int(nullable: false, identity: true),
                        CollectionItemID = c.Int(nullable: false),
                        Latitude = c.String(nullable: false),
                        Longitude = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PlacedItemID)
                .ForeignKey("dbo.CollectionItems", t => t.CollectionItemID, cascadeDelete: true)
                .Index(t => t.CollectionItemID);

            CreateTable(
                "dbo.CollectionItems",
                c => new
                    {
                        CollectionItemID = c.Int(nullable: false, identity: true),
                        CompanyID = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.CollectionItemID)
                .ForeignKey("dbo.Companies", t => t.CompanyID, cascadeDelete: true)
                .Index(t => t.CompanyID);

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
                .PrimaryKey(t => t.CompanyID)
                .ForeignKey("dbo.EneoUsers", t => t.UserManagerID)
                .Index(t => t.UserManagerID);

            CreateTable(
                "dbo.RefreshTokens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Subject = c.String(nullable: false, maxLength: 50),
                        ClientId = c.String(nullable: false, maxLength: 50),
                        IssuedUtc = c.DateTime(nullable: false),
                        ExpiresUtc = c.DateTime(nullable: false),
                        ProtectedTicket = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");

            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");

            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);

            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
        }

        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.PlacedItems", "CollectionItemID", "dbo.CollectionItems");
            DropForeignKey("dbo.CollectionItems", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Companies", "UserManagerID", "dbo.EneoUsers");
            DropForeignKey("dbo.CollectedItems", "PlacedItemID", "dbo.PlacedItems");
            DropForeignKey("dbo.CollectedItems", "EneoUserID", "dbo.EneoUsers");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Companies", new[] { "UserManagerID" });
            DropIndex("dbo.CollectionItems", new[] { "CompanyID" });
            DropIndex("dbo.PlacedItems", new[] { "CollectionItemID" });
            DropIndex("dbo.CollectedItems", new[] { "PlacedItemID" });
            DropIndex("dbo.CollectedItems", new[] { "EneoUserID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.RefreshTokens");
            DropTable("dbo.Companies");
            DropTable("dbo.CollectionItems");
            DropTable("dbo.PlacedItems");
            DropTable("dbo.EneoUsers");
            DropTable("dbo.CollectedItems");
            DropTable("dbo.Clients");
        }
    }
}