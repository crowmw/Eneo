namespace Eneo.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastposition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EneoUsers", "LastLatitude", c => c.String());
            AddColumn("dbo.EneoUsers", "LastLongitude", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EneoUsers", "LastLongitude");
            DropColumn("dbo.EneoUsers", "LastLatitude");
        }
    }
}
