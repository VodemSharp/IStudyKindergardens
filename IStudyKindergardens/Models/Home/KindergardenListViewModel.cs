using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Models
{
    public class KindergardenListViewModel
    {
        public List<Kindergarden> Kindergardens { get; set; }
        public List<string> Addresses { get; set; }
        public List<string> PreviewPictures { get; set; }
        public List<string> ShortInfo { get; set; }
    }
}