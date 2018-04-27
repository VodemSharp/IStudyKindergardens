using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Kindergartens
{
    public class ChangeAddressViewModel
    {
        public string Id { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина адреси - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть адресу закладу!")]
        public string Address { get; set; }

        [MaxLength(100, ErrorMessage = "Максимальна довжина альтернативної адреси - 100 символів!")]
        [Required(ErrorMessage = "Вкажіть альтернативну адресу закладу!")]
        public string AltAddress { get; set; }
    }
}