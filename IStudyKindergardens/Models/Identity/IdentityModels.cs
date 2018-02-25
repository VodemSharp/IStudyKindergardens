using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IStudyKindergardens.Models
{
    //Time standard: '00:00:00 dd/mm/yyyy'

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual SiteUser SiteUser { get; set; }
        public virtual Kindergarden Kindergarden { get; set; }

        public ApplicationUser()
        {
            IdentityUser u = new IdentityUser();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class SiteUser
    {
        [Key, ForeignKey("ApplicationUser")]
        public string Id { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public string Surname { get; set; }
        public string Name { get; set; }
        public string FathersName { get; set; }
        public string DateOfBirth { get; set; }

        public ICollection<SiteUserClaim> SiteUserClaims { get; set; }

        public SiteUser()
        {
            SiteUserClaims = new List<SiteUserClaim>();
        }
    }

    public class SiteUserClaim
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        [ForeignKey("ClaimType")]
        public int ClaimTypeId { get; set; }
        public ClaimType ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public class Kindergarden
    {
        [Key, ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }

        public ICollection<DescriptionBlock> DescriptionBlocks { get; set; }
        public ICollection<KindergardenClaim> KindergardenClaims { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public Kindergarden()
        {
            DescriptionBlocks = new List<DescriptionBlock>();
            KindergardenClaims = new List<KindergardenClaim>();
        }
    }

    public class KindergardenClaim
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarden")]
        public string KindergardenId { get; set; }
        public Kindergarden Kindergarden { get; set; }

        [ForeignKey("ClaimType")]
        public int ClaimTypeId { get; set; }
        public ClaimType ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public abstract class DescriptionBlock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarden")]
        public string KindergardenId { get; set; }
        public Kindergarden Kindergarden { get; set; }

        [NotMapped]
        public virtual string BlockType { get; }
        [NotMapped]
        public virtual List<string> BlockComponents { get; set; }
    }

    public class DescriptionBlockText : DescriptionBlock
    {
        public string Header { get; set; }
        public string Body { get; set; }

        [NotMapped]
        public override string BlockType
        {
            get
            {
                return "Text";
                //return "<div class='description-block-custom'><h4>"+Header+"</h4><p>"+Body+"</p></div>";
            }
        }

        [NotMapped]
        public override List<string> BlockComponents
        {
            get
            {
                return new List<string> { Header, Body };
            }
            set
            {
                if (value[0] != null)
                {
                    Header = value[0];
                }
                if (value[1] != null)
                {
                    Body = value[1];
                }
            }
        }
    }

    public class DescriptionBlockTextImage : DescriptionBlock
    {
        public string Image { get; set; }
        public string Header { get; set; }
        public string Body { get; set; }

        [NotMapped]
        public override string BlockType
        {
            get
            {
                return "TextImage";
                //return "<div class='description-image-block-custom'><div><img id='preview-image-image' class='img-rounded img-responsive' style='width: 25 %;' src='/Images/Uploaded/Source/"+ Image + "><h4>"+Header+"</h4><h5>"+Body+"</h5></div></div>";
            }
        }

        [NotMapped]
        public override List<string> BlockComponents
        {
            get
            {
                return new List<string> { Image, Header, Body };
            }
            set
            {
                if (value[0] != null)
                {
                    Image = value[0];
                }
                if (value[1] != null)
                {
                    Header = value[1];
                }
                if (value[2] != null)
                {
                    Body = value[2];
                }
            }
        }
    }

    public class ClaimType
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }

        public ICollection<SiteUserClaim> SiteUserClaims { get; set; }
        public ICollection<KindergardenClaim> KindergardenClaims { get; set; }

        public ClaimType()
        {
            SiteUserClaims = new List<SiteUserClaim>();
            KindergardenClaims = new List<KindergardenClaim>();
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<SiteUserClaim> SiteUserClaims { get; set; }

        public DbSet<Kindergarden> Kindergardens { get; set; }
        public DbSet<KindergardenClaim> KindergardenClaims { get; set; }
        public DbSet<DescriptionBlock> DescriptionBlocks { get; set; }

        public DbSet<ClaimType> ClaimTypes { get; set; }

        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}