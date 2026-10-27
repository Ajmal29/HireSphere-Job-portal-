using System;
using System.Collections.Generic;

namespace JobSearchApp_Mvc_Project.Models
{
    public class Jobsearch
    {
        public Jobsearch()
        {
            selectjob = new List<jobList>();
            insertse = new jobList();
        }

        public jobList insertse { get; set; }
        public List<jobList> selectjob { get; set; }
    }

    public class jobList
    {
        public int Job_id { get; set; }
        public int Company_id { get; set; }
        public string Job_Tittle { get; set; }
        public string Job_description { get; set; }
        public string Job_Experience { get; set; }
        public string Job_Skills { get; set; }
        public string Job_Salary { get; set; }
        public DateTime Job_enddate { get; set; }
        public string Job_Location { get; set; }
        public string Job_Status { get; set; }
        public int jobtype_Id { get; set; }
        public string jobtype_name { get; set; }
        public string Qualification { get { return jobtype_name; } set { jobtype_name = value; } }
    }
}
