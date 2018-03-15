namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddMessages : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        MessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.MessageId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApplicationUserMessages", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.Messages", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ApplicationUserMessages", "ApplicationUserId", "dbo.AspNetUsers");
            DropIndex("dbo.Messages", new[] { "ApplicationUserId" });
            DropIndex("dbo.ApplicationUserMessages", new[] { "MessageId" });
            DropIndex("dbo.ApplicationUserMessages", new[] { "ApplicationUserId" });
            DropTable("dbo.Messages");
            DropTable("dbo.ApplicationUserMessages");
        }
    }
}
