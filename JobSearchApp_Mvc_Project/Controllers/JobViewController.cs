using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using JobSearchApp_Mvc_Project.Models;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class JobViewController : Controller
    {
        // GET: JobView/jobview_pageload
        public ActionResult jobview_pageload()
        {
            Jobsearch clsobj = new Jobsearch();
            return View("jobview_pageload", getdata(clsobj, ""));
        }

        // POST: JobView/searchjob_click
        [HttpPost]
        public ActionResult searchjob_click(Jobsearch clsobj)
        {
            string qry = "";

            if (clsobj != null && clsobj.insertse != null)
            {
                if (!string.IsNullOrWhiteSpace(clsobj.insertse.Job_Experience))
                {
                    qry += " and ExperienceYears like '%" + clsobj.insertse.Job_Experience.Trim() + "%'";
                }
                if (!string.IsNullOrWhiteSpace(clsobj.insertse.Job_Skills))
                {
                    qry += " and Skills like '%" + clsobj.insertse.Job_Skills.Trim() + "%'";
                }
                if (!string.IsNullOrWhiteSpace(clsobj.insertse.Job_Location))
                {
                    qry += " and Location like '%" + clsobj.insertse.Job_Location.Trim() + "%'";
                }
            }

            return View("jobview_pageload", getdata(clsobj, qry));
        }

        private Jobsearch getdata(Jobsearch clsobj, string qry)
        {
            string connString = ConfigurationManager.ConnectionStrings["importdataconnection"] != null
                ? ConfigurationManager.ConnectionStrings["importdataconnection"].ConnectionString
                : "Data Source=DESKTOP-QGFB0P7\\SQLEXPRESS;Initial Catalog=MVCProjectJobSearch;Integrated Security=True;";

            using (var con = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand("sp_Jobsearches", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@qry", qry ?? "");
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                var joblist = new Jobsearch();

                if (clsobj != null && clsobj.insertse != null)
                {
                    joblist.insertse = clsobj.insertse;
                }

                while (dr.Read())
                {
                    var jobcls = new jobList();

      
                    if (dr.HasColumn("Job_id") && dr["Job_id"] != DBNull.Value)
                        jobcls.Job_id = Convert.ToInt32(dr["Job_id"]);
                    else if (dr.HasColumn("JobId") && dr["JobId"] != DBNull.Value)
                        jobcls.Job_id = Convert.ToInt32(dr["JobId"]);

  
                    if (dr.HasColumn("Company_id") && dr["Company_id"] != DBNull.Value)
                        jobcls.Company_id = Convert.ToInt32(dr["Company_id"]);
                    else if (dr.HasColumn("CompanyId") && dr["CompanyId"] != DBNull.Value)
                        jobcls.Company_id = Convert.ToInt32(dr["CompanyId"]);

                    if (dr.HasColumn("Job_Tittle") && dr["Job_Tittle"] != DBNull.Value)
                        jobcls.Job_Tittle = dr["Job_Tittle"].ToString();
                    else if (dr.HasColumn("JobTitle") && dr["JobTitle"] != DBNull.Value)
                        jobcls.Job_Tittle = dr["JobTitle"].ToString();

      
                    if (dr.HasColumn("Job_description") && dr["Job_description"] != DBNull.Value)
                        jobcls.Job_description = dr["Job_description"].ToString();
                    else if (dr.HasColumn("JobTitle") && dr["JobTitle"] != DBNull.Value)
                        jobcls.Job_description = dr["JobTitle"].ToString();

       
                    if (dr.HasColumn("Job_Type") && dr["Job_Type"] != DBNull.Value)
                        jobcls.jobtype_name = dr["Job_Type"].ToString();
                    else if (dr.HasColumn("Qualification") && dr["Qualification"] != DBNull.Value)
                        jobcls.jobtype_name = dr["Qualification"].ToString();


                    if (dr.HasColumn("Job_Experience") && dr["Job_Experience"] != DBNull.Value)
                        jobcls.Job_Experience = dr["Job_Experience"].ToString();
                    else if (dr.HasColumn("ExperienceYears") && dr["ExperienceYears"] != DBNull.Value)
                        jobcls.Job_Experience = dr["ExperienceYears"].ToString();

     
                    if (dr.HasColumn("Job_Skills") && dr["Job_Skills"] != DBNull.Value)
                        jobcls.Job_Skills = dr["Job_Skills"].ToString();
                    else if (dr.HasColumn("Skills") && dr["Skills"] != DBNull.Value)
                        jobcls.Job_Skills = dr["Skills"].ToString();

        
                    if (dr.HasColumn("Job_Salary") && dr["Job_Salary"] != DBNull.Value)
                        jobcls.Job_Salary = dr["Job_Salary"].ToString();
                    else
                        jobcls.Job_Salary = "Competitive";

             
                    if (dr.HasColumn("Job_enddate") && dr["Job_enddate"] != DBNull.Value)
                        jobcls.Job_enddate = Convert.ToDateTime(dr["Job_enddate"]);
                    else if (dr.HasColumn("EndDate") && dr["EndDate"] != DBNull.Value)
                        jobcls.Job_enddate = Convert.ToDateTime(dr["EndDate"]);
                    else
                        jobcls.Job_enddate = DateTime.Today.AddDays(30);

        
                    if (dr.HasColumn("Job_Location") && dr["Job_Location"] != DBNull.Value)
                        jobcls.Job_Location = dr["Job_Location"].ToString();
                    else if (dr.HasColumn("Location") && dr["Location"] != DBNull.Value)
                        jobcls.Job_Location = dr["Location"].ToString();

        
                    if (dr.HasColumn("Job_Status") && dr["Job_Status"] != DBNull.Value)
                        jobcls.Job_Status = dr["Job_Status"].ToString();
                    else if (dr.HasColumn("JobStatus") && dr["JobStatus"] != DBNull.Value)
                        jobcls.Job_Status = dr["JobStatus"].ToString();

                    joblist.selectjob.Add(jobcls);
                }
                con.Close();
                return joblist;
            }
        }
    }
}
