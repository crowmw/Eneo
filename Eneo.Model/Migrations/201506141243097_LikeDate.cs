namespace Eneo.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LikeDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DMPFacebookLikes", "AddedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.DMPFacebookLikes", "AddedDate");
        }
    }
}
