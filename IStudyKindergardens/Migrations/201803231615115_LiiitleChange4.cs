namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LiiitleChange4 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MessageReMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageId = c.Int(nullable: false),
                        ReMessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .ForeignKey("dbo.ReMessages", t => t.ReMessageId, cascadeDelete: true)
                .Index(t => t.MessageId)
                .Index(t => t.ReMessageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MessageReMessages", "ReMessageId", "dbo.ReMessages");
            DropForeignKey("dbo.MessageReMessages", "MessageId", "dbo.Messages");
            DropIndex("dbo.MessageReMessages", new[] { "ReMessageId" });
            DropIndex("dbo.MessageReMessages", new[] { "MessageId" });
            DropTable("dbo.MessageReMessages");
        }
    }
}
