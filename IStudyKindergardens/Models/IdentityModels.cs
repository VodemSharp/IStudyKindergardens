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
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Post> Posts { get; set; }

        public SiteUser()
        {
            SiteUserClaims = new List<SiteUserClaim>();
            Ratings = new List<Rating>();
            Comments = new List<Comment>();
            Posts = new List<Post>();
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

    public class ClaimType
    {
        [Key]
        public int Id { get; set; }

        public string Type { get; set; }

        public ICollection<SiteUserClaim> SiteUserClaims { get; set; }

        public ClaimType()
        {
            SiteUserClaims = new List<SiteUserClaim>();
        }
    }

    public class DescriptionBlock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarden")]
        public int KindergardenId { get; set; }
        public Kindergarden Kindergarden { get; set; }

        public string Head { get; set; }
        public string Body { get; set; }
    }

    public class Kindergarden
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public ICollection<DescriptionBlock> DescriptionBlocks { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Post> Posts { get; set; }

        public Kindergarden()
        {
            DescriptionBlocks = new List<DescriptionBlock>();
            Ratings = new List<Rating>();
            Posts = new List<Post>();
        }
    }

    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        [ForeignKey("Kindergarden")]
        public int KindergardenId { get; set; }
        public Kindergarden Kindergarden { get; set; }

        public int Value { get; set; }
    }

    public class Post
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        [ForeignKey("Kindergarden")]
        public int KindergardenId { get; set; }
        public Kindergarden Kindergarden { get; set; }

        public string Text { get; set; }
        public DateTime Time { get; set; }
    }

    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; }

        public string Text { get; set; }
        public DateTime Time { get; set; }
    }

    public class TempPicture
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
        public DateTime Time { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<DescriptionBlock> DescriptionBlocks { get; set; }
        public DbSet<Kindergarden> Kindergardens { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<TempPicture> TempPictures { get; set; }
        public DbSet<SiteUserClaim> SiteUserClaims { get; set; }
        public DbSet<ClaimType> ClaimTypes { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}