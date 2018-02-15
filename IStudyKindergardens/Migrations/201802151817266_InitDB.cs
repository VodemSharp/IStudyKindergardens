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
                        Email = c.String(),
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
                "dbo.DescriptionBlocks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KindergardenId = c.String(maxLength: 128),
                        Head = c.String(),
                        Body = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Kindergardens", t => t.KindergardenId)
                .Index(t => t.KindergardenId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.KindergardenClaims", "KindergardenId", "dbo.Kindergardens");
            DropForeignKey("dbo.DescriptionBlocks", "KindergardenId", "dbo.Kindergardens");
            DropForeignKey("dbo.Kindergardens", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.SiteUserClaims", "SiteUserId", "dbo.SiteUsers");
            DropForeignKey("dbo.SiteUserClaims", "ClaimTypeId", "dbo.ClaimTypes");
            DropForeignKey("dbo.SiteUsers", "Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.KindergardenClaims", "ClaimTypeId", "dbo.ClaimTypes");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.DescriptionBlocks", new[] { "KindergardenId" });
            DropIndex("dbo.SiteUserClaims", new[] { "ClaimTypeId" });
            DropIndex("dbo.SiteUserClaims", new[] { "SiteUserId" });
            DropIndex("dbo.SiteUsers", new[] { "Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Kindergardens", new[] { "Id" });
            DropIndex("dbo.KindergardenClaims", new[] { "ClaimTypeId" });
            DropIndex("dbo.KindergardenClaims", new[] { "KindergardenId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.DescriptionBlocks");
            DropTable("dbo.SiteUserClaims");
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
