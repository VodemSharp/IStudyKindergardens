namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LCH1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId);
            
            CreateTable(
                "dbo.SiteUserContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                        ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId, cascadeDelete: true)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId)
                .Index(t => t.ContactId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteUserContacts", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.SiteUserContacts", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "SiteUserId", "dbo.SiteUsers");
            DropIndex("dbo.SiteUserContacts", new[] { "ContactId" });
            DropIndex("dbo.SiteUserContacts", new[] { "SiteUserId" });
            DropIndex("dbo.Contacts", new[] { "SiteUserId" });
            DropTable("dbo.SiteUserContacts");
            DropTable("dbo.Contacts");
        }
    }
}
