namespace Eneo.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class likeID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DMPFacebookLikes", "LikeID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.DMPFacebookLikes", "LikeID");
        }
    }
}
