using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Messages
{
    public class SendMessageToViewModel
    {
        public string Theme { set; get; }
        [Required(ErrorMessage = "Виберіть користувача!")]
        public string ToUserId { get; set; }
        public string ToUser { get; set; }
        [Required(ErrorMessage = "Введіть текст повідомлення!")]
        public string Text { get; set; }
    }
}