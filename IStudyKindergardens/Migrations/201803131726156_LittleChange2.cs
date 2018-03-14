namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleChange2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.KindergardenStatements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergardenId = c.String(maxLength: 128),
                        SiteUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergardens", t => t.KindergardenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.KindergardenId)
                .Index(t => t.SiteUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.KindergardenStatements", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.KindergardenStatements", "KindergardenId", "dbo.Kindergardens");
            DropIndex("dbo.KindergardenStatements", new[] { "SiteUserId" });
            DropIndex("dbo.KindergardenStatements", new[] { "KindergardenId" });
            DropTable("dbo.KindergardenStatements");
        }
    }
}
