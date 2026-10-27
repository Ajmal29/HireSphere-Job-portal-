using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JobSearchApp_Mvc_Project.Models
{
    public class UserInsert
    {

        [Required(ErrorMessage = "Enter the name")]
        public string name { get; set; }

        [Required(ErrorMessage = "Enter the age")]
        public int? age { get; set; }

        [Required(ErrorMessage = "Enter the address")]
        public string address { get; set; }

        [Required(ErrorMessage = "Enter the phone")]
        [RegularExpression(@"^(\d{10})$",
            ErrorMessage = "Enter valid number")]
        public string phone { get; set; }

        [Required(ErrorMessage = "Enter the email")]
        [EmailAddress(ErrorMessage = "Enter valid email id")]
        public string email { get; set; }

        [Required(ErrorMessage = "Enter the qualification")]
        public string qualification { get; set; }

        [Required(ErrorMessage = "Enter the experience")]
        public decimal? experience { get; set; }

        [Required(ErrorMessage = "Enter the skills")]
        public string skills { get; set; }

        public string photo { get; set; }

        public int status { get; set; }

        [Required(ErrorMessage = "Enter the username")]
        public string username { get; set; }

        [Required(ErrorMessage = "Enter the password")]
        public string pass { get; set; }

        [Required(ErrorMessage = "Confirm the password")]
        [Compare("pass", ErrorMessage = "Password mismatch")]
        public string cpassword { get; set; }

        public string usermsg { get; set; }
    }
}