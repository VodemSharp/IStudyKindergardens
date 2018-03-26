using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Models.Messages
{
    public class SendMessageViewModel
    {
        public string Theme { set; get; }
        [Required(ErrorMessage = "Виберіть користувача!")]
        public string ToUserId { get; set; }
        [Required(ErrorMessage = "Введіть текст повідомлення!")]
        public string Text { get; set; }
        public SelectList UserContacts { get; set; }
    }
}