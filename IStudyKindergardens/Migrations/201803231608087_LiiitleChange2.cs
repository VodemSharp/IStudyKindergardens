namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LiiitleChange2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Messages", "ReMessageId", "dbo.Messages");
            DropIndex("dbo.Messages", new[] { "ReMessageId" });
            DropColumn("dbo.Messages", "ReMessageId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "ReMessageId", c => c.Int());
            CreateIndex("dbo.Messages", "ReMessageId");
            AddForeignKey("dbo.Messages", "ReMessageId", "dbo.Messages", "Id");
        }
    }
}
