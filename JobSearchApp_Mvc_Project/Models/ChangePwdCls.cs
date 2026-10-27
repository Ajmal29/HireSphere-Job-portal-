using System;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace JobSearchApp_Mvc_Project.Models
{
    public class ChangePwdCls
    {
        public string OldPassword { set; get; }
        [Required(ErrorMessage = "Enter the password")]
        public string NewPassword { set; get; }
        [Compare("NewPassword", ErrorMessage = " password Mismatch")]
        public string ConfirmPassword { set; get; }
        public string msg { set; get; }
    }
}
