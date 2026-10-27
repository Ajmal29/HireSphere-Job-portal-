using System;
using System.Collections.Generic;

namespace JobSearchApp_Mvc_Project.Models
{
    public class Applicationcls
    {
        public Applicationcls()
        {
            selectjobdetails = new List<jobdetails>();
        }

        public int User_id { get; set; }
        public int Company_id { get; set; }
        public int Job_id { get; set; }
        public string Resume { get; set; }
        public DateTime App_Date { get; set; }
        public string App_Status { get; set; }
        public List<jobdetails> selectjobdetails { get; set; }
    }

    public class jobdetails
    {
        public int Company_idFK { get; set; }
        public string Job_Tittle { get; set; }
        public string Job_description { get; set; }
        public string jobtype_name { get; set; }
        public string Job_Experience { get; set; }
        public string Job_Skills { get; set; }
        public string Job_Salary { get; set; }
        public DateTime Job_enddate { get; set; }
        public string Job_Location { get; set; }
    }
}
