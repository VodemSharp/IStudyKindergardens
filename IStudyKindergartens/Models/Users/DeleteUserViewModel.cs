using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Users
{
    public class DeleteUserViewModel
    {
        [Required]
        public string Id { get; set; }
    }
}