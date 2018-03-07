namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRating : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergardenId = c.String(maxLength: 128),
                        SiteUserId = c.String(maxLength: 128),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergardens", t => t.KindergardenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.KindergardenId)
                .Index(t => t.SiteUserId);
            
            CreateTable(
                "dbo.QuestionRatings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuestionRatingRatings",
                c => new
                    {
                        QuestionRating_Id = c.Int(nullable: false),
                        Rating_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuestionRating_Id, t.Rating_Id })
                .ForeignKey("dbo.QuestionRatings", t => t.QuestionRating_Id, cascadeDelete: true)
                .ForeignKey("dbo.Ratings", t => t.Rating_Id, cascadeDelete: true)
                .Index(t => t.QuestionRating_Id)
                .Index(t => t.Rating_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Ratings", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.QuestionRatingRatings", "Rating_Id", "dbo.Ratings");
            DropForeignKey("dbo.QuestionRatingRatings", "QuestionRating_Id", "dbo.QuestionRatings");
            DropForeignKey("dbo.QuestionRatings", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Ratings", "KindergardenId", "dbo.Kindergardens");
            DropIndex("dbo.QuestionRatingRatings", new[] { "Rating_Id" });
            DropIndex("dbo.QuestionRatingRatings", new[] { "QuestionRating_Id" });
            DropIndex("dbo.QuestionRatings", new[] { "QuestionId" });
            DropIndex("dbo.Ratings", new[] { "SiteUserId" });
            DropIndex("dbo.Ratings", new[] { "KindergardenId" });
            DropTable("dbo.QuestionRatingRatings");
            DropTable("dbo.Questions");
            DropTable("dbo.QuestionRatings");
            DropTable("dbo.Ratings");
        }
    }
}
