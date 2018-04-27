using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace IStudyKindergartens.HelpClasses
{
    public static class MailCustom
    {
        public static bool Mail(string email, string header, string content)
        {
            try
            {
                MailMessage msg = new MailMessage(Email, email, header, content)
                {
                    IsBodyHtml = true
                };
                SmtpClient sc = new SmtpClient("smtp.gmail.com", 587)
                {
                    UseDefaultCredentials = false
                };
                NetworkCredential cre = new NetworkCredential(Email, Password);
                sc.Credentials = cre;
                sc.EnableSsl = true;
                sc.Send(msg);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region Properties

        private static string UserName
        {
            get
            {
                return "IStudy";
            }
        }

        private static string Email
        {
            get
            {
                return "istudy.network@gmail.com";
            }
        }

        private static string Password
        {
            get
            {
                return "istudyrepublika";
            }
        }

        #endregion
    }
}