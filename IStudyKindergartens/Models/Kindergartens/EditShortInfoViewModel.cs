using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Kindergartens
{
    public class EditShortInfoViewModel
    {
        public string Id { get; set; }

        [MaxLength(350, ErrorMessage = "Максимальна довжина інформації - 350 символів!")]
        public string ShortInfo { get; set; }
    }
}