using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IStudyKindergartens.Models.Statements
{
    public class AddStatementViewModel
    {
        [Required(ErrorMessage = "Вкажіть своє П.І.П.!")]
        [Display(Name = "SNF")]
        public string SNF { get; set; }

        [Required(ErrorMessage = "Вкажіть серію, номер паспорта одного з батьків!")]
        [Display(Name = "SeriesNumberPassport")]
        public string SeriesNumberPassport { get; set; }

        [Required(ErrorMessage = "Вкажіть П.І.П. дитини!")]
        [Display(Name = "ChildSNF")]
        public string ChildSNF { get; set; }

        [Required(ErrorMessage = "Вкажіть свою дату народження!")]
        [Display(Name = "CheckDateOfBirth")]
        public string ChildDateOfBirth { get; set; }

        [Required(ErrorMessage = "Вкажіть серію, номер свідоцтва про народження дитини!")]
        [Display(Name = "ChildBirthCertificate")]
        public string ChildBirthCertificate { get; set; }

        [Required(ErrorMessage = "Виберіть дошкільний навчальний заклад!")]
        [Display(Name = "SelectedKindergartenId")]
        public string SelectedKindergartenId { get; set; }

        [Display(Name = "Kindergartens")]
        public SelectList Kindergartens { get; set; }

        [Required(ErrorMessage = "Вкажіть адресу проживання!")]
        [Display(Name = "KindergartenId")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Вкажіть свій електронний адрес!")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не коректно вказаний емейл!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Вкажіть свій номер телефону!")]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(Name = "AdditionalPhoneNumber")]
        public string AdditionalPhoneNumber { get; set; }

        [Display(Name = "Privileges")]
        public List<PrivilegesInnerViewModel> Privileges { get; set; }

        [Display(Name = "Groups")]
        public List<string> Groups { get; set; }

        [Required(ErrorMessage = "Виберіть тип групи!")]
        [Display(Name = "SelectedGroup")]
        public string SelectedGroup { get; set; }

        [Required(ErrorMessage = "Підтвердіть згоду на використання персональних даних!")]
        [Display(Name = "Consent")]
        public bool Consent { get; set; }

        [Required(ErrorMessage = "Введіть код перевірки!")]
        [Display(Name = "Captcha")]
        public string Captcha { get; set; }
    }
}