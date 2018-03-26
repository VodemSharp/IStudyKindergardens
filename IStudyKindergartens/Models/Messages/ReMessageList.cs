using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Messages
{
    public class ReMessageList
    {
        public string ReceiverId { get; set; }
        public bool IsUser { get; set; }
        public string Theme { get; set; }
        public int ReMessageId { get; set; }
        [Required]
        public string NewText { get; set; }
        public List<ReMessageItem> ReMessages { get; set; }
    }
}