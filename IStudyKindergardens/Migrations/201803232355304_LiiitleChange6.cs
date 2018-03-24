namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LiiitleChange6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "IsHiddenForReciver", c => c.Boolean(nullable: false));
            AddColumn("dbo.Messages", "IsHiddenForSender", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "IsHiddenForSender");
            DropColumn("dbo.Messages", "IsHiddenForReciver");
        }
    }
}
