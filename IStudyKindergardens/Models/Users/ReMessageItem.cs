using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Models
{
    public class ReMessageItem
    {
        public string SenderId { get; set; }
        public string Sender { get; set; }
        public bool IsUser { get; set; }
        [Required]
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
    }
}