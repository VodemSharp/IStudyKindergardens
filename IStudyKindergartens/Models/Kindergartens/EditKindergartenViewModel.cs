using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Kindergartens
{
    public class EditKindergartenViewModel
    {
        [Display(Name="Id")]
        public string Id { get; set; }

        [Display(Name = "PictureName")]
        public string PictureName { get; set; }

        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "Вкажіть назву закладу!")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "Вкажіть адресу закладу!")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Вкажіть електронний адрес закладу!")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не коректно вказаний емейл!")]
        public string Email { get; set; }

        public List<DescriptionBlock> DescriptionBlocks { get; set; }
    }
}