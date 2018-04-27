using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Models.Kindergartens
{
    public class EditKindergartenViewModel
    {
        public string Id { get; set; }

        public string PictureName { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина назви закладу - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть назву закладу!")]
        public string Name { get; set; }

        public List<DescriptionBlock> DescriptionBlocks { get; set; }
    }
}