using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Models
{
    public class MessageUserListItemModel
    {
        public string From { get; set; }
        public string FromId { get; set; }
        public bool IsFromUser { get; set; }
        public int MessageId { get; set; }
        public string Theme { get; set; }
        public string Text { get; set; }
        public bool IsRead { get; set; }
        public DateTime DateTime { get; set; }
    }
}