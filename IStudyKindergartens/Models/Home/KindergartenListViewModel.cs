using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Home
{
    public class KindergartenListItemViewModel
    {
        public Kindergarten Kindergarten { get; set; }
        public string Address { get; set; }
        public string PreviewPicture { get; set; }
        public string ShortInfo { get; set; }
        public string Rating { get; set; }
        public bool IsSelected { get; set; }
    }
}