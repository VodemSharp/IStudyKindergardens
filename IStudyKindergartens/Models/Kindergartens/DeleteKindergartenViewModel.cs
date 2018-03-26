using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Kindergartens
{
    public class DeleteKindergartenViewModel
    {
        [Required]
        public string Id { get; set; }
        public string Name { get; set; } 
    }
}