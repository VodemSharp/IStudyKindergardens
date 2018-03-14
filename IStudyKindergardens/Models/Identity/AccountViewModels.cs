using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace IStudyKindergardens.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "Вкажіть своє прізвище!")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "Вкажіть своє ім'я!")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "Вкажіть своє ім'я по батькові!")]
        [Display(Name = "FathersName")]
        public string FathersName { get; set; }

        [Required(ErrorMessage = "Вкажіть свій номер телефону!")]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Вкажіть свій електронний адрес!")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не коректно вказаний емейл!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Вкажіть свою дату народження!")]
        [Display(Name = "DateOfBirth")]
        public string DateOfBirth { get; set; }

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
