using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Models.Users
{
    public class AddUserViewModel
    {
        public string PictureName { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина прізвища - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть своє прізвище!")]
        public string Surname { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина імені - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть своє ім'я!")]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина імені по батькові - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть своє ім'я по батькові!")]
        public string FathersName { get; set; }

        [Required(ErrorMessage = "Вкажіть свій номер телефону!")]
        [RegularExpression(@"\([0-9]{3}\)\ [0-9]{3}\-[0-9]{4}", ErrorMessage = "Формат телефону: '(000) 000-0000'!")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Вкажіть свій електронний адрес!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не коректно вказаний email!")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Не коректно вказаний email!")]
        [Remote("IsEmailExist", "Home")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Вкажіть свою дату народження!")]
        [DataType(DataType.Date, ErrorMessage = "Не коректно вказана дата народження!")]
        [Remote("CheckDate", "Home")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Вкажіть пароль!")]
        [MaxLength(100, ErrorMessage = "Максимальна довжина паролю - 100 символів!")]
        [MinLength(6, ErrorMessage = "Пароль повинен бути більше 6 символів!")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Повторіть пароль!")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Паролі не співпадають!")]
        public string ConfirmPassword { get; set; }
    }
}