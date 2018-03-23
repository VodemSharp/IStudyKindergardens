namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LiiitleChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "ReMessageId", c => c.Int());
            CreateIndex("dbo.Messages", "ReMessageId");
            AddForeignKey("dbo.Messages", "ReMessageId", "dbo.Messages", "Id");
            DropColumn("dbo.Messages", "ReMessage");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "ReMessage", c => c.Int());
            DropForeignKey("dbo.Messages", "ReMessageId", "dbo.Messages");
            DropIndex("dbo.Messages", new[] { "ReMessageId" });
            DropColumn("dbo.Messages", "ReMessageId");
        }
    }
}
