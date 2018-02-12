namespace IStudyKindergardens.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LittleChange5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteUserClaims", "ClaimValue", c => c.String());
            DropColumn("dbo.SiteUserClaims", "ValueClaim");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SiteUserClaims", "ValueClaim", c => c.String());
            DropColumn("dbo.SiteUserClaims", "ClaimValue");
        }
    }
}
