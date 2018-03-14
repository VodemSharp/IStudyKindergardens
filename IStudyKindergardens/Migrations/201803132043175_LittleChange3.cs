namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleChange3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Statements", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Statements", "DateTime");
        }
    }
}
