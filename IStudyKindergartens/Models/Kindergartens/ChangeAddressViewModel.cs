using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Kindergartens
{
    public class ChangeAddressViewModel
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Вкажіть адресу закладу!")]
        [Display(Name = "KindergartenAddress")]
        public string Address { get; set; }

        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "Вкажіть альтернативну адресу закладу!")]
        [Display(Name = "KindergartenAltAddress")]
        public string AltAddress { get; set; }
    }
}