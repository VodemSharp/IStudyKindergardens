using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergardens.Models
{
    public class AddKindergardenViewModel
    {
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

        [Required(ErrorMessage = "Вкажіть пароль!")]
        [StringLength(100, ErrorMessage = "Пароль повинен бути більше 6 символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Повторіть пароль!")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Паролі не співпадають!")]
        public string ConfirmPassword { get; set; }
    }
}