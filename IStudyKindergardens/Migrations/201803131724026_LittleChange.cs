namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Statements", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Statements", "Status");
        }
    }
}
