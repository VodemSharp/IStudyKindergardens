namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TimeReform : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TempPictures", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Comments", "Time", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Posts", "Time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Posts", "Time", c => c.String());
            AlterColumn("dbo.Comments", "Time", c => c.String());
            DropColumn("dbo.TempPictures", "Time");
        }
    }
}
