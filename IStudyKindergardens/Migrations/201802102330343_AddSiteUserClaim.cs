namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteUserClaim : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                        Key = c.String(),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteUserClaims", "SiteUserId", "dbo.SiteUsers");
            DropIndex("dbo.SiteUserClaims", new[] { "SiteUserId" });
            DropTable("dbo.SiteUserClaims");
        }
    }
}
