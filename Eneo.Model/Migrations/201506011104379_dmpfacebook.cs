namespace Eneo.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dmpfacebook : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DMPFacebooks",
                c => new
                    {
                        DMPFacebookID = c.Int(nullable: false, identity: true),
                        EneoUserID = c.String(nullable: false, maxLength: 128),
                        FacebookID = c.Int(nullable: false),
                        Email = c.String(),
                        Gender = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Language = c.String(),
                        Locale = c.String(),
                        Timezone = c.Int(nullable: false),
                        Verified = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DMPFacebookID)
                .ForeignKey("dbo.EneoUsers", t => t.EneoUserID, cascadeDelete: true)
                .Index(t => t.EneoUserID);
            
            CreateTable(
                "dbo.DMPFacebookLikes",
                c => new
                    {
                        DMPFacebookLikesID = c.Int(nullable: false, identity: true),
                        DMPFacebookID = c.Int(nullable: false),
                        CategoryName = c.String(),
                        LikeName = c.String(),
                    })
                .PrimaryKey(t => t.DMPFacebookLikesID)
                .ForeignKey("dbo.DMPFacebooks", t => t.DMPFacebookID, cascadeDelete: true)
                .Index(t => t.DMPFacebookID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DMPFacebooks", "EneoUserID", "dbo.EneoUsers");
            DropForeignKey("dbo.DMPFacebookLikes", "DMPFacebookID", "dbo.DMPFacebooks");
            DropIndex("dbo.DMPFacebookLikes", new[] { "DMPFacebookID" });
            DropIndex("dbo.DMPFacebooks", new[] { "EneoUserID" });
            DropTable("dbo.DMPFacebookLikes");
            DropTable("dbo.DMPFacebooks");
        }
    }
}
