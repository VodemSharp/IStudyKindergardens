using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergardens.Models
{
    public class StatementListItemViewModel
    {
        public Statement Statement { get; set; }
        public string KindergardenName { get; set; }
        public string UserName { get; set; }
        public List<string> UserPrivileges { get; set; }
    }
}