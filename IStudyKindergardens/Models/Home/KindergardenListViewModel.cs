using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Models
{
    public class KindergardenListItemViewModel
    {
        public Kindergarden Kindergarden { get; set; }
        public string Address { get; set; }
        public string PreviewPicture { get; set; }
        public string ShortInfo { get; set; }
        public string Rating { get; set; }
        public bool IsSelected { get; set; }
    }
}