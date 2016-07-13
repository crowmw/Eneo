namespace Eneo.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addDeviceInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DMPDeviceInfoes",
                c => new
                    {
                        DMPDeviceInfoID = c.Int(nullable: false, identity: true),
                        EneoUserID = c.String(maxLength: 128),
                        OperatingSystem = c.String(),
                        SystemFirmwareVersion = c.String(),
                        SystemHardwareVersion = c.String(),
                        SystemManufacturer = c.String(),
                        SystemProductName = c.String(),
                    })
                .PrimaryKey(t => t.DMPDeviceInfoID)
                .ForeignKey("dbo.EneoUsers", t => t.EneoUserID)
                .Index(t => t.EneoUserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DMPDeviceInfoes", "EneoUserID", "dbo.EneoUsers");
            DropIndex("dbo.DMPDeviceInfoes", new[] { "EneoUserID" });
            DropTable("dbo.DMPDeviceInfoes");
        }
    }
}
