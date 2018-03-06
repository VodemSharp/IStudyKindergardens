using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Models
{
    public class ChangeAddressViewModel
    {
        [Display(Name = "Id")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Вкажіть адресу закладу!")]
        [Display(Name = "KindergardenAddress")]
        public string Address { get; set; }

        [StringLength(maximumLength: 100)]
        [Required(ErrorMessage = "Вкажіть альтернативну адресу закладу!")]
        [Display(Name = "KindergardenAltAddress")]
        public string AltAddress { get; set; }
    }
}