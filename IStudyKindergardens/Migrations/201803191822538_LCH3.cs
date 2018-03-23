namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LCH3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "IsRead", c => c.Boolean(nullable: false));
            AddColumn("dbo.Messages", "Theme", c => c.String());
            AddColumn("dbo.Messages", "Text", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "Text");
            DropColumn("dbo.Messages", "Theme");
            DropColumn("dbo.Messages", "IsRead");
        }
    }
}
