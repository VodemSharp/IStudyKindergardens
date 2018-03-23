namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Messages", "ReMessage", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Messages", "ReMessage");
        }
    }
}
