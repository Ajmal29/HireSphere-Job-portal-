using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobSearchApp_Mvc_Project.Models
{
    public class LoginClass
    {
        [Required(ErrorMessage = "Enter username")]
        public string username { get; set; }

        [Required(ErrorMessage = "Enter password")]
        public string password { get; set; }

        public string msg { get; set; }
    }
}