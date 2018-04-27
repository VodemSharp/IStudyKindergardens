using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Models.Kindergartens
{
    public class AddKindergartenViewModel
    {
        public string PictureName { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина назви - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть назву закладу!")]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина адреси - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть адресу закладу!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Вкажіть електронний адрес закладу!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не коректно вказаний email!")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Не коректно вказаний email!")]
        public string Email { get; set; }
    }
}