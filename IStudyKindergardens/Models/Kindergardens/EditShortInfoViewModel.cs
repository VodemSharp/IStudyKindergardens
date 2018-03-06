using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Models
{
    public class EditShortInfoViewModel
    {
        public string Id { get; set; }

        [Display(Name = "ShortInfo")]
        [StringLength(350, ErrorMessage = "So long...")]
        public string ShortInfo { get; set; }
    }
}