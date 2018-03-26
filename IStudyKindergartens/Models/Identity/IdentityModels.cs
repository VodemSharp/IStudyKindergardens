using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace IStudyKindergartens.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public virtual SiteUser SiteUser { get; set; }
        public virtual Kindergarten Kindergarten { get; set; }

        public ICollection<Message> Messages { get; set; }
        public ICollection<ApplicationUserMessage> ApplicationUserMessages { get; set; }

        public ApplicationUser()
        {
            IdentityUser u = new IdentityUser();
            Messages = new List<Message>();
            ApplicationUserMessages = new List<ApplicationUserMessage>();
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

        [NotMapped]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1} {2}", Surname, Name, FathersName);
            }
        }

        public ICollection<SiteUserClaim> SiteUserClaims { get; set; }
        public ICollection<SiteUserKindergarten> SiteUserKindergartens { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Statement> Statements { get; set; }
        public ICollection<Contact> Contacts { get; set; }
        public ICollection<SiteUserContact> SiteUserContact { get; set; }

        public SiteUser()
        {
            SiteUserClaims = new List<SiteUserClaim>();
            SiteUserKindergartens = new List<SiteUserKindergarten>();
            Ratings = new List<Rating>();
            Statements = new List<Statement>();
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

    public class SiteUserContact
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        [ForeignKey("Contact")]
        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }

    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        public ICollection<SiteUserContact> SiteUserContact { get; set; }

        public Contact()
        {
            SiteUserContact = new List<SiteUserContact>();
        }
    }

    public class ApplicationUserMessage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("Message")]
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }

    public class Message
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public bool IsRead { get; set; }
        public bool IsHiddenForReciver { get; set; }
        public bool IsHiddenForSender { get; set; }
        public string Theme { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }

        public virtual ReMessage ReMessage { get; set; }

        public ICollection<MessageReMessage> MessageReMessages { get; set; }
        public ICollection<ApplicationUserMessage> ApplicationUserMessages { get; set; }

        public Message()
        {
            ApplicationUserMessages = new List<ApplicationUserMessage>();
        }
    }

    public class MessageReMessage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Message")]
        public int MessageId { get; set; }
        public Message Message { get; set; }

        [ForeignKey("ReMessage")]
        public int ReMessageId { get; set; }
        public ReMessage ReMessage { get; set; }
    }

    public class ReMessage
    {
        [Key, ForeignKey("Message")]
        public int Id { get; set; }

        public virtual Message Message { get; set; }
        public ICollection<MessageReMessage> MessageReMessages { get; set; }
    }

    public class SiteUserKindergarten
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        [ForeignKey("Kindergarten")]
        public string KindergartenId { get; set; }
        public Kindergarten Kindergarten { get; set; }
    }

    public class Kindergarten
    {
        [Key, ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double ActualRating { get; set; }

        public ICollection<DescriptionBlock> DescriptionBlocks { get; set; }
        public ICollection<KindergartenClaim> KindergartenClaims { get; set; }
        public ICollection<SiteUserKindergarten> SiteUserKindergartens { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<Statement> Statements { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public Kindergarten()
        {
            DescriptionBlocks = new List<DescriptionBlock>();
            KindergartenClaims = new List<KindergartenClaim>();
            SiteUserKindergartens = new List<SiteUserKindergarten>();
            Ratings = new List<Rating>();
            Statements = new List<Statement>();
        }
    }

    public class KindergartenClaim
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarten")]
        public string KindergartenId { get; set; }
        public Kindergarten Kindergarten { get; set; }

        [ForeignKey("ClaimType")]
        public int ClaimTypeId { get; set; }
        public ClaimType ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    public abstract class DescriptionBlock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarten")]
        public string KindergartenId { get; set; }
        public Kindergarten Kindergarten { get; set; }

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
        public ICollection<KindergartenClaim> KindergartenClaims { get; set; }

        public ClaimType()
        {
            SiteUserClaims = new List<SiteUserClaim>();
            KindergartenClaims = new List<KindergartenClaim>();
        }
    }

    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarten")]
        public string KindergartenId { get; set; }
        public Kindergarten Kindergarten { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        public string Comment { get; set; }

        public virtual ICollection<QuestionRating> QuestionRatings { get; set; }

        public Rating()
        {
            QuestionRatings = new HashSet<QuestionRating>();
        }
    }

    public class QuestionRating
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public Question Question { get; set; }

        public int Rating { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }

        public QuestionRating()
        {
            Ratings = new HashSet<Rating>();
        }
    }

    public class Question
    {
        [Key]
        public int Id { get; set; }

        public string Value { get; set; }

        public ICollection<QuestionRating> QuestionRatings { get; set; }

        public Question()
        {
            QuestionRatings = new List<QuestionRating>();
        }
    }

    public class Statement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarten")]
        public string KindergartenId { get; set; }
        public Kindergarten Kindergarten { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }

        public string SNF { get; set; }
        public string SeriesNumberPassport { get; set; }
        public string ChildSNF { get; set; }
        public string ChildDateOfBirth { get; set; }
        public string ChildBirthCertificate { get; set; }
        public string Group { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        public string Status { get; set; }
        public bool IsRemoved { get; set; }
        public bool IsSelected { get; set; }
        public DateTime DateTime { get; set; }

        public ICollection<UserPrivilegeStatement> UserPrivilegeStatements { get; set; }

        public Statement()
        {
            UserPrivilegeStatements = new List<UserPrivilegeStatement>();
        }
    }

    public class KindergartenStatement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Kindergarten")]
        public string KindergartenId { get; set; }
        public Kindergarten Kindergarten { get; set; }

        [ForeignKey("SiteUser")]
        public string SiteUserId { get; set; }
        public SiteUser SiteUser { get; set; }
    }

    public class UserPrivilegeStatement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserPrivilege")]
        public int UserPrivilegeId { get; set; }
        public UserPrivilege UserPrivilege { get; set; }

        [ForeignKey("Statement")]
        public int StatementId { get; set; }
        public Statement Statement { get; set; }
    }

    public class UserPrivilege
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }

        public ICollection<UserPrivilegeStatement> UserPrivilegeStatements { get; set; }

        public UserPrivilege()
        {
            UserPrivilegeStatements = new List<UserPrivilegeStatement>();
        }
    }

    public class Group
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class Privilege
    {
        [Key]
        public int Id { get; set; }
        public string Value { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<SiteUser> SiteUsers { get; set; }
        public DbSet<SiteUserClaim> SiteUserClaims { get; set; }
        public DbSet<SiteUserKindergarten> SiteUserKindergartens { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<SiteUserContact> SiteUserContacts { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<ApplicationUserMessage> ApplicationUserMessages { get; set; }
        public DbSet<ReMessage> ReMessages { get; set; }
        public DbSet<MessageReMessage> MessageReMessages { get; set; }

        public DbSet<Kindergarten> Kindergartens { get; set; }
        public DbSet<KindergartenClaim> KindergartenClaims { get; set; }
        public DbSet<DescriptionBlock> DescriptionBlocks { get; set; }
        public DbSet<DescriptionBlockText> DescriptionBlocksText { get; set; }
        public DbSet<DescriptionBlockTextImage> DescriptionBlocksTextImage { get; set; }
        public DbSet<KindergartenStatement> KindergartenStatements { get; set; }

        public DbSet<Statement> Statements { get; set; }
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserPrivilege> UserPrivileges { get; set; }
        public DbSet<UserPrivilegeStatement> UserPrivilegeStatements { get; set; }

        public DbSet<Rating> Ratings { get; set; }
        public DbSet<QuestionRating> QuestionRatings { get; set; }
        public DbSet<Question> Questions { get; set; }

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