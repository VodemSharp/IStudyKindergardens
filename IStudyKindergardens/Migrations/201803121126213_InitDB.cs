namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ClaimTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.KindergardenClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergardenId = c.String(maxLength: 128),
                        ClaimTypeId = c.Int(nullable: false),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClaimTypes", t => t.ClaimTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Kindergardens", t => t.KindergardenId)
                .Index(t => t.KindergardenId)
                .Index(t => t.ClaimTypeId);
            
            CreateTable(
                "dbo.Kindergardens",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Address = c.String(),
                        ActualRating = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.SiteUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Surname = c.String(),
                        Name = c.String(),
                        FathersName = c.String(),
                        DateOfBirth = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id)
                .Index(t => t.Id);
            
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
                "dbo.SiteUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                        ClaimTypeId = c.Int(nullable: false),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClaimTypes", t => t.ClaimTypeId, cascadeDelete: true)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId)
                .Index(t => t.ClaimTypeId);
            
            CreateTable(
                "dbo.SiteUserKindergardens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                        KindergardenId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergardens", t => t.KindergardenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId)
                .Index(t => t.KindergardenId);
            
            CreateTable(
                "dbo.Statements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergardenId = c.String(maxLength: 128),
                        SiteUserId = c.String(maxLength: 128),
                        SNF = c.String(),
                        SeriesNumberPassport = c.String(),
                        ChildSNF = c.String(),
                        ChildDateOfBirth = c.String(),
                        ChildBirthCertificate = c.String(),
                        Group = c.String(),
                        Address = c.String(),
                        Email = c.String(),
                        PhoneNumber = c.String(),
                        AdditionalPhoneNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergardens", t => t.KindergardenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.KindergardenId)
                .Index(t => t.SiteUserId);
            
            CreateTable(
                "dbo.UserPrivilegeStatements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserPrivilegeId = c.Int(nullable: false),
                        StatementId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Statements", t => t.StatementId, cascadeDelete: true)
                .ForeignKey("dbo.UserPrivileges", t => t.UserPrivilegeId, cascadeDelete: true)
                .Index(t => t.UserPrivilegeId)
                .Index(t => t.StatementId);
            
            CreateTable(
                "dbo.UserPrivileges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DescriptionBlocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergardenId = c.String(maxLength: 128),
                        Header = c.String(),
                        Body = c.String(),
                        Image = c.String(),
                        Header1 = c.String(),
                        Body1 = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergardens", t => t.KindergardenId)
                .Index(t => t.KindergardenId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Privileges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
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
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.KindergardenClaims", "KindergardenId", "dbo.Kindergardens");
            DropForeignKey("dbo.DescriptionBlocks", "KindergardenId", "dbo.Kindergardens");
            DropForeignKey("dbo.Kindergardens", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserPrivilegeStatements", "UserPrivilegeId", "dbo.UserPrivileges");
            DropForeignKey("dbo.UserPrivilegeStatements", "StatementId", "dbo.Statements");
            DropForeignKey("dbo.Statements", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.Statements", "KindergardenId", "dbo.Kindergardens");
            DropForeignKey("dbo.SiteUserKindergardens", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.SiteUserKindergardens", "KindergardenId", "dbo.Kindergardens");
            DropForeignKey("dbo.SiteUserClaims", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.SiteUserClaims", "ClaimTypeId", "dbo.ClaimTypes");
            DropForeignKey("dbo.Ratings", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.QuestionRatingRatings", "Rating_Id", "dbo.Ratings");
            DropForeignKey("dbo.QuestionRatingRatings", "QuestionRating_Id", "dbo.QuestionRatings");
            DropForeignKey("dbo.QuestionRatings", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Ratings", "KindergardenId", "dbo.Kindergardens");
            DropForeignKey("dbo.SiteUsers", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.KindergardenClaims", "ClaimTypeId", "dbo.ClaimTypes");
            DropIndex("dbo.QuestionRatingRatings", new[] { "Rating_Id" });
            DropIndex("dbo.QuestionRatingRatings", new[] { "QuestionRating_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.DescriptionBlocks", new[] { "KindergardenId" });
            DropIndex("dbo.UserPrivilegeStatements", new[] { "StatementId" });
            DropIndex("dbo.UserPrivilegeStatements", new[] { "UserPrivilegeId" });
            DropIndex("dbo.Statements", new[] { "SiteUserId" });
            DropIndex("dbo.Statements", new[] { "KindergardenId" });
            DropIndex("dbo.SiteUserKindergardens", new[] { "KindergardenId" });
            DropIndex("dbo.SiteUserKindergardens", new[] { "SiteUserId" });
            DropIndex("dbo.SiteUserClaims", new[] { "ClaimTypeId" });
            DropIndex("dbo.SiteUserClaims", new[] { "SiteUserId" });
            DropIndex("dbo.QuestionRatings", new[] { "QuestionId" });
            DropIndex("dbo.Ratings", new[] { "SiteUserId" });
            DropIndex("dbo.Ratings", new[] { "KindergardenId" });
            DropIndex("dbo.SiteUsers", new[] { "Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Kindergardens", new[] { "Id" });
            DropIndex("dbo.KindergardenClaims", new[] { "ClaimTypeId" });
            DropIndex("dbo.KindergardenClaims", new[] { "KindergardenId" });
            DropTable("dbo.QuestionRatingRatings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Privileges");
            DropTable("dbo.Groups");
            DropTable("dbo.DescriptionBlocks");
            DropTable("dbo.UserPrivileges");
            DropTable("dbo.UserPrivilegeStatements");
            DropTable("dbo.Statements");
            DropTable("dbo.SiteUserKindergardens");
            DropTable("dbo.SiteUserClaims");
            DropTable("dbo.Questions");
            DropTable("dbo.QuestionRatings");
            DropTable("dbo.Ratings");
            DropTable("dbo.SiteUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Kindergardens");
            DropTable("dbo.KindergardenClaims");
            DropTable("dbo.ClaimTypes");
        }
    }
}
