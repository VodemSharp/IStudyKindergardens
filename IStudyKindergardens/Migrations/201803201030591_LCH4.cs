namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LCH4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "DateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "DateTime");
        }
    }
}
