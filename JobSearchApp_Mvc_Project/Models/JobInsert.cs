using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobSearchApp_Mvc_Project.Models
{
    public class JobInsert
    {
        [Required(ErrorMessage = "Enter Job Title")]
        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }


        [Required(ErrorMessage = "Enter Job Experience")]
        [Display(Name = "Job Experience")]
        public string JobExperience { get; set; }

        public string Skills { get; set; }

        public string Qualification { get; set; }

        [Required(ErrorMessage = "Enter Job Location")]
        [Display(Name = "Job Location")]
        public string JobLocation { get; set; }

        [Required(ErrorMessage = "Select Start Date")]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "Select End Date")]
        [Display(Name = "End Date")]
        public string EndDate { get; set; }

        [Required(ErrorMessage = "Enter Vacancies")]
        [Display(Name = "Vacancies")]
        public int? Vacancies { get; set; }

        public string[] selectedQual { get; set; }
        public List<CheckBoxListHelper> MyFavouriteQual { get; set; }

        public string[] selectedSkill { get; set; }
        public List<CheckBoxListHelper> MyFavouriteSkill { get; set; }

        public string JobStatus { get; set; }
        public string msg { get; set; }

        public string Location { get { return JobLocation; } set { JobLocation = value; } }
        public string ExperienceYears { get { return JobExperience; } set { JobExperience = value; } }
    }
}

