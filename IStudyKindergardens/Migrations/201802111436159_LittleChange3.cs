namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleChange3 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.TypeClaims", newName: "ClaimTypes");
            RenameColumn(table: "dbo.SiteUserClaims", name: "TypeClaimId", newName: "ClaimTypeId");
            RenameIndex(table: "dbo.SiteUserClaims", name: "IX_TypeClaimId", newName: "IX_ClaimTypeId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.SiteUserClaims", name: "IX_ClaimTypeId", newName: "IX_TypeClaimId");
            RenameColumn(table: "dbo.SiteUserClaims", name: "ClaimTypeId", newName: "TypeClaimId");
            RenameTable(name: "dbo.ClaimTypes", newName: "TypeClaims");
        }
    }
}
