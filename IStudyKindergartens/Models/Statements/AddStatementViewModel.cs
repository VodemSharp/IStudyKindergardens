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
        public string SNF { get; set; }

        [Required(ErrorMessage = "Вкажіть серію, номер паспорта одного з батьків!")]
        public string SeriesNumberPassport { get; set; }

        [Required(ErrorMessage = "Вкажіть П.І.П. дитини!")]
        public string ChildSNF { get; set; }

        [Required(ErrorMessage = "Вкажіть дату народження дитини!")]
        [DataType(DataType.Date, ErrorMessage = "Не коректно вказана дата народження!")]
        [Remote("CheckDate", "Home")]
        public string DateOfBirth { get; set; }

        [Required(ErrorMessage = "Вкажіть серію, номер свідоцтва про народження дитини!")]
        public string ChildBirthCertificate { get; set; }

        [Required(ErrorMessage = "Виберіть дошкільний навчальний заклад!")]
        public string SelectedKindergartenId { get; set; }

        public SelectList Kindergartens { get; set; }

        [Required(ErrorMessage = "Вкажіть адресу проживання!")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Вкажіть свій електронний адрес!")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не коректно вказаний email!")]
        [RegularExpression(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-‌​]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$", ErrorMessage = "Не коректно вказаний email!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Вкажіть свій номер телефону!")]
        [RegularExpression(@"\([0-9]{3}\)\ [0-9]{3}\-[0-9]{4}", ErrorMessage = "Формат телефону: '(000) 000-0000'!")]
        public string PhoneNumber { get; set; }

        public string AdditionalPhoneNumber { get; set; }

        public List<PrivilegesInnerViewModel> Privileges { get; set; }

        public List<string> Groups { get; set; }

        [Required(ErrorMessage = "Виберіть тип групи!")]
        public string SelectedGroup { get; set; }

        public bool Consent { get; set; }

        [Required(ErrorMessage = "Введіть код перевірки!")]
        public string Captcha { get; set; }
    }
}