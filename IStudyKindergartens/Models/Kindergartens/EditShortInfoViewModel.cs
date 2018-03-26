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

        [Display(Name = "ShortInfo")]
        [StringLength(350, ErrorMessage = "So long...")]
        public string ShortInfo { get; set; }
    }
}