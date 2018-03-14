namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleChange4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Statements", "IsRemoved", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Statements", "IsRemoved");
        }
    }
}
