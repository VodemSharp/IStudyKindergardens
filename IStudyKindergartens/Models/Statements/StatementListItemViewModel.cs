using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IStudyKindergartens.Models.Statements
{
    public class StatementListItemViewModel
    {
        public Statement Statement { get; set; }
        public string KindergartenName { get; set; }
        public string UserName { get; set; }
        public List<string> UserPrivileges { get; set; }
    }
}