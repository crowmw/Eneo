namespace Eneo.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lastpositiondouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.EneoUsers", "LastLatitude", c => c.Double());
            AlterColumn("dbo.EneoUsers", "LastLongitude", c => c.Double());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EneoUsers", "LastLongitude", c => c.String());
            AlterColumn("dbo.EneoUsers", "LastLatitude", c => c.String());
        }
    }
}
