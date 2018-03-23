namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LiiitleChange3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReMessages",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.Id)
                .Index(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReMessages", "Id", "dbo.Messages");
            DropIndex("dbo.ReMessages", new[] { "Id" });
            DropTable("dbo.ReMessages");
        }
    }
}
