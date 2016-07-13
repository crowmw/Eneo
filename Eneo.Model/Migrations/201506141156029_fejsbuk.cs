namespace Eneo.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fejsbuk : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DMPFacebooks", "FacebookID", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DMPFacebooks", "FacebookID", c => c.Int(nullable: false));
        }
    }
}
