namespace IStudyKindergartens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApplicationUserMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        MessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.MessageId);
            
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
                "dbo.Kindergartens",
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
                "dbo.DescriptionBlocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergartenId = c.String(maxLength: 128),
                        Header = c.String(),
                        Body = c.String(),
                        Image = c.String(),
                        Header1 = c.String(),
                        Body1 = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergartens", t => t.KindergartenId)
                .Index(t => t.KindergartenId);
            
            CreateTable(
                "dbo.KindergartenClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergartenId = c.String(maxLength: 128),
                        ClaimTypeId = c.Int(nullable: false),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ClaimTypes", t => t.ClaimTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Kindergartens", t => t.KindergartenId)
                .Index(t => t.KindergartenId)
                .Index(t => t.ClaimTypeId);
            
            CreateTable(
                "dbo.ClaimTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
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
                "dbo.Contacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId);
            
            CreateTable(
                "dbo.SiteUserContacts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                        ContactId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contacts", t => t.ContactId, cascadeDelete: true)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId)
                .Index(t => t.ContactId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergartenId = c.String(maxLength: 128),
                        SiteUserId = c.String(maxLength: 128),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergartens", t => t.KindergartenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.KindergartenId)
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
                "dbo.SiteUserKindergartens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SiteUserId = c.String(maxLength: 128),
                        KindergartenId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergartens", t => t.KindergartenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.SiteUserId)
                .Index(t => t.KindergartenId);
            
            CreateTable(
                "dbo.Statements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergartenId = c.String(maxLength: 128),
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
                        Status = c.String(),
                        IsRemoved = c.Boolean(nullable: false),
                        IsSelected = c.Boolean(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergartens", t => t.KindergartenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.KindergartenId)
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
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.String(maxLength: 128),
                        IsRead = c.Boolean(nullable: false),
                        IsHiddenForReciver = c.Boolean(nullable: false),
                        IsHiddenForSender = c.Boolean(nullable: false),
                        Theme = c.String(),
                        Text = c.String(),
                        DateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId)
                .Index(t => t.ApplicationUserId);
            
            CreateTable(
                "dbo.MessageReMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageId = c.Int(nullable: false),
                        ReMessageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.MessageId, cascadeDelete: true)
                .ForeignKey("dbo.ReMessages", t => t.ReMessageId, cascadeDelete: true)
                .Index(t => t.MessageId)
                .Index(t => t.ReMessageId);
            
            CreateTable(
                "dbo.ReMessages",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Messages", t => t.Id)
                .Index(t => t.Id);
            
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
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.KindergartenStatements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergartenId = c.String(maxLength: 128),
                        SiteUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergartens", t => t.KindergartenId)
                .ForeignKey("dbo.SiteUsers", t => t.SiteUserId)
                .Index(t => t.KindergartenId)
                .Index(t => t.SiteUserId);
            
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
            DropForeignKey("dbo.KindergartenStatements", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.KindergartenStatements", "KindergartenId", "dbo.Kindergartens");
            DropForeignKey("dbo.ApplicationUserMessages", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.ApplicationUserMessages", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageReMessages", "ReMessageId", "dbo.ReMessages");
            DropForeignKey("dbo.ReMessages", "Id", "dbo.Messages");
            DropForeignKey("dbo.MessageReMessages", "MessageId", "dbo.Messages");
            DropForeignKey("dbo.Messages", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.KindergartenClaims", "KindergartenId", "dbo.Kindergartens");
            DropForeignKey("dbo.KindergartenClaims", "ClaimTypeId", "dbo.ClaimTypes");
            DropForeignKey("dbo.SiteUserClaims", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.UserPrivilegeStatements", "UserPrivilegeId", "dbo.UserPrivileges");
            DropForeignKey("dbo.UserPrivilegeStatements", "StatementId", "dbo.Statements");
            DropForeignKey("dbo.Statements", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.Statements", "KindergartenId", "dbo.Kindergartens");
            DropForeignKey("dbo.SiteUserKindergartens", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.SiteUserKindergartens", "KindergartenId", "dbo.Kindergartens");
            DropForeignKey("dbo.Ratings", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.QuestionRatingRatings", "Rating_Id", "dbo.Ratings");
            DropForeignKey("dbo.QuestionRatingRatings", "QuestionRating_Id", "dbo.QuestionRatings");
            DropForeignKey("dbo.QuestionRatings", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Ratings", "KindergartenId", "dbo.Kindergartens");
            DropForeignKey("dbo.SiteUserContacts", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.SiteUserContacts", "ContactId", "dbo.Contacts");
            DropForeignKey("dbo.Contacts", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.SiteUsers", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SiteUserClaims", "ClaimTypeId", "dbo.ClaimTypes");
            DropForeignKey("dbo.DescriptionBlocks", "KindergartenId", "dbo.Kindergartens");
            DropForeignKey("dbo.Kindergartens", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.QuestionRatingRatings", new[] { "Rating_Id" });
            DropIndex("dbo.QuestionRatingRatings", new[] { "QuestionRating_Id" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.KindergartenStatements", new[] { "SiteUserId" });
            DropIndex("dbo.KindergartenStatements", new[] { "KindergartenId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.ReMessages", new[] { "Id" });
            DropIndex("dbo.MessageReMessages", new[] { "ReMessageId" });
            DropIndex("dbo.MessageReMessages", new[] { "MessageId" });
            DropIndex("dbo.Messages", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.UserPrivilegeStatements", new[] { "StatementId" });
            DropIndex("dbo.UserPrivilegeStatements", new[] { "UserPrivilegeId" });
            DropIndex("dbo.Statements", new[] { "SiteUserId" });
            DropIndex("dbo.Statements", new[] { "KindergartenId" });
            DropIndex("dbo.SiteUserKindergartens", new[] { "KindergartenId" });
            DropIndex("dbo.SiteUserKindergartens", new[] { "SiteUserId" });
            DropIndex("dbo.QuestionRatings", new[] { "QuestionId" });
            DropIndex("dbo.Ratings", new[] { "SiteUserId" });
            DropIndex("dbo.Ratings", new[] { "KindergartenId" });
            DropIndex("dbo.SiteUserContacts", new[] { "ContactId" });
            DropIndex("dbo.SiteUserContacts", new[] { "SiteUserId" });
            DropIndex("dbo.Contacts", new[] { "SiteUserId" });
            DropIndex("dbo.SiteUsers", new[] { "Id" });
            DropIndex("dbo.SiteUserClaims", new[] { "ClaimTypeId" });
            DropIndex("dbo.SiteUserClaims", new[] { "SiteUserId" });
            DropIndex("dbo.KindergartenClaims", new[] { "ClaimTypeId" });
            DropIndex("dbo.KindergartenClaims", new[] { "KindergartenId" });
            DropIndex("dbo.DescriptionBlocks", new[] { "KindergartenId" });
            DropIndex("dbo.Kindergartens", new[] { "Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ApplicationUserMessages", new[] { "MessageId" });
            DropIndex("dbo.ApplicationUserMessages", new[] { "ApplicationUserId" });
            DropTable("dbo.QuestionRatingRatings");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Privileges");
            DropTable("dbo.KindergartenStatements");
            DropTable("dbo.Groups");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.ReMessages");
            DropTable("dbo.MessageReMessages");
            DropTable("dbo.Messages");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.UserPrivileges");
            DropTable("dbo.UserPrivilegeStatements");
            DropTable("dbo.Statements");
            DropTable("dbo.SiteUserKindergartens");
            DropTable("dbo.Questions");
            DropTable("dbo.QuestionRatings");
            DropTable("dbo.Ratings");
            DropTable("dbo.SiteUserContacts");
            DropTable("dbo.Contacts");
            DropTable("dbo.SiteUsers");
            DropTable("dbo.SiteUserClaims");
            DropTable("dbo.ClaimTypes");
            DropTable("dbo.KindergartenClaims");
            DropTable("dbo.DescriptionBlocks");
            DropTable("dbo.Kindergartens");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ApplicationUserMessages");
        }
    }
}
