namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleChange2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TypeClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.SiteUserClaims", "TypeClaimId", c => c.Int(nullable: false));
            AddColumn("dbo.SiteUserClaims", "ValueClaim", c => c.String());
            CreateIndex("dbo.SiteUserClaims", "TypeClaimId");
            AddForeignKey("dbo.SiteUserClaims", "TypeClaimId", "dbo.TypeClaims", "Id", cascadeDelete: true);
            DropColumn("dbo.SiteUserClaims", "Key");
            DropColumn("dbo.SiteUserClaims", "Value");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SiteUserClaims", "Value", c => c.String());
            AddColumn("dbo.SiteUserClaims", "Key", c => c.String());
            DropForeignKey("dbo.SiteUserClaims", "TypeClaimId", "dbo.TypeClaims");
            DropIndex("dbo.SiteUserClaims", new[] { "TypeClaimId" });
            DropColumn("dbo.SiteUserClaims", "ValueClaim");
            DropColumn("dbo.SiteUserClaims", "TypeClaimId");
            DropTable("dbo.TypeClaims");
        }
    }
}
