using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IStudyKindergartens.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Вкажіть свій електронний адрес!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не коректно вказаний email!")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Не коректно вказаний email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Вкажіть свій пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
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

    //public class ExternalLoginConfirmationViewModel
    //{
    //    [Required]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}
    //
    //public class ExternalLoginListViewModel
    //{
    //    public string ReturnUrl { get; set; }
    //}
    //
    //public class SendCodeViewModel
    //{
    //    public string SelectedProvider { get; set; }
    //    public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    //    public string ReturnUrl { get; set; }
    //    public bool RememberMe { get; set; }
    //}
    //
    //public class VerifyCodeViewModel
    //{
    //    [Required]
    //    public string Provider { get; set; }
    //
    //    [Required]
    //    [Display(Name = "Code")]
    //    public string Code { get; set; }
    //    public string ReturnUrl { get; set; }
    //
    //    [Display(Name = "Remember this browser?")]
    //    public bool RememberBrowser { get; set; }
    //
    //    public bool RememberMe { get; set; }
    //}
    //
    //public class ForgotViewModel
    //{
    //    [Required]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}

    //public class ResetPasswordViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //
    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }
    //
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm password")]
    //    [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }
    //
    //    public string Code { get; set; }
    //}
    //
    //public class ForgotPasswordViewModel
    //{
    //    [Required]
    //    [EmailAddress]
    //    [Display(Name = "Email")]
    //    public string Email { get; set; }
    //}
}
