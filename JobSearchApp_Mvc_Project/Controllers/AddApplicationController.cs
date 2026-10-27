using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchApp_Mvc_Project.Models;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class AddApplicationController : Controller
    {
        // GET: AddApplication/Apllication_load
        public ActionResult Apllication_load(int jid, int cid)
        {
            TempData["cid"] = cid;
            Session["Jid"] = jid;
            TempData["jid"] = jid;

            int userid = Session["uid"] != null ? Convert.ToInt32(Session["uid"]) : 0;
            int jobids = Convert.ToInt32(Session["Jid"]);
            int countapply = 0;

            string connString = ConfigurationManager.ConnectionStrings["importdataconnection"] != null
                ? ConfigurationManager.ConnectionStrings["importdataconnection"].ConnectionString
                : "server=.;database=MVCProjectJobSearch;integrated security=true;";

            using (var con = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand("SP_CountUserAppliedJobWithID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@userid", userid);
                cmd.Parameters.AddWithValue("@Jobid", jobids);
                SqlParameter opParam = new SqlParameter("@status", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(opParam);
                con.Open();
                cmd.ExecuteNonQuery();
                if (opParam.Value != null && opParam.Value != DBNull.Value)
                {
                    countapply = Convert.ToInt32(opParam.Value);
                }
                con.Close();
            }

            if (countapply == 1)
            {
                ViewBag.isapplied = countapply;
            }

            if (userid > 0)
            {
                using (var db = new JobSearchApp_Mvc_Project.MVCProjectJobSearchEntities())
                {
                    ViewBag.CandidateProfile = db.sp_profile(userid).FirstOrDefault();
                }
            }

            return View(getdata());
        }

        // POST: AddApplication/Application_click
        [HttpPost]
        public ActionResult Application_click(HttpPostedFileBase file, Applicationcls clsobj)
        {
            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    string fname = Path.GetFileName(file.FileName);
                    var s = Server.MapPath("~/ApplicationResume");
                    if (!Directory.Exists(s))
                    {
                        Directory.CreateDirectory(s);
                    }
                    string pa = Path.Combine(s, fname);
                    file.SaveAs(pa);
                    var fullpath = Path.Combine("~/ApplicationResume", fname);
                    clsobj.Resume = fullpath;
                }
                else
                {
                    clsobj.Resume = "";
                }

                DateTime currentDate = DateTime.Today;
                clsobj.App_Date = currentDate;
                clsobj.User_id = Session["uid"] != null ? Convert.ToInt32(Session["uid"]) : 0;
                clsobj.Company_id = TempData["cid"] != null ? Convert.ToInt32(TempData["cid"]) : 0;
                int jobid = Session["Jid"] != null ? Convert.ToInt32(Session["Jid"]) : (TempData["jid"] != null ? Convert.ToInt32(TempData["jid"]) : 0);

                string connString = ConfigurationManager.ConnectionStrings["importdataconnection"] != null
                    ? ConfigurationManager.ConnectionStrings["importdataconnection"].ConnectionString
                    : "server=.;database=MVCProjectJobSearch;integrated security=true;";

                using (var con = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("SP_PostApplication", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@User_id", clsobj.User_id);
                    cmd.Parameters.AddWithValue("@Company_id", clsobj.Company_id);
                    cmd.Parameters.AddWithValue("@Job_Id", jobid);
                    cmd.Parameters.AddWithValue("@Resume", clsobj.Resume ?? "");
                    cmd.Parameters.AddWithValue("@App_Date", clsobj.App_Date);
                    cmd.Parameters.AddWithValue("@App_Status", "apply");
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                TempData["SuccessMessage"] = "Job application submitted successfully!";
                return RedirectToAction("jobview_pageload", "JobView");
            }
            else
            {
                return View("Apllication_load", clsobj);
            }
        }

        private Applicationcls getdata()
        {
            string connString = ConfigurationManager.ConnectionStrings["importdataconnection"] != null
                ? ConfigurationManager.ConnectionStrings["importdataconnection"].ConnectionString
                : "server=.;database=MVCProjectJobSearch;integrated security=true;";

            using (var con = new SqlConnection(connString))
            {
                SqlCommand cmd = new SqlCommand("SP_GetJobDatawithID", con);
                cmd.CommandType = CommandType.StoredProcedure;
                var jidValue = TempData["jid"] ?? Session["Jid"] ?? 0;
                cmd.Parameters.AddWithValue("@jobid", Convert.ToInt32(jidValue));
                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                var joblist = new Applicationcls();

                while (dr.Read())
                {
                    var jobcls = new jobdetails();

                    // Read Company ID
                    if (dr.HasColumn("Company_id") && dr["Company_id"] != DBNull.Value)
                        jobcls.Company_idFK = Convert.ToInt32(dr["Company_id"]);
                    else if (dr.HasColumn("CompanyId") && dr["CompanyId"] != DBNull.Value)
                        jobcls.Company_idFK = Convert.ToInt32(dr["CompanyId"]);

                    // Read Job Title
                    if (dr.HasColumn("Job_Tittle") && dr["Job_Tittle"] != DBNull.Value)
                        jobcls.Job_Tittle = dr["Job_Tittle"].ToString();
                    else if (dr.HasColumn("JobTitle") && dr["JobTitle"] != DBNull.Value)
                        jobcls.Job_Tittle = dr["JobTitle"].ToString();

                    // Read Job Description
                    if (dr.HasColumn("Job_description") && dr["Job_description"] != DBNull.Value)
                        jobcls.Job_description = dr["Job_description"].ToString();
                    else if (dr.HasColumn("JobTitle") && dr["JobTitle"] != DBNull.Value)
                        jobcls.Job_description = dr["JobTitle"].ToString();

                    // Read Job Type
                    if (dr.HasColumn("Job_Type") && dr["Job_Type"] != DBNull.Value)
                        jobcls.jobtype_name = dr["Job_Type"].ToString();
                    else if (dr.HasColumn("Qualification") && dr["Qualification"] != DBNull.Value)
                        jobcls.jobtype_name = dr["Qualification"].ToString();

                    // Read Experience
                    if (dr.HasColumn("Job_Experience") && dr["Job_Experience"] != DBNull.Value)
                        jobcls.Job_Experience = dr["Job_Experience"].ToString();
                    else if (dr.HasColumn("ExperienceYears") && dr["ExperienceYears"] != DBNull.Value)
                        jobcls.Job_Experience = dr["ExperienceYears"].ToString();

                    // Read Skills
                    if (dr.HasColumn("Job_Skills") && dr["Job_Skills"] != DBNull.Value)
                        jobcls.Job_Skills = dr["Job_Skills"].ToString();
                    else if (dr.HasColumn("Skills") && dr["Skills"] != DBNull.Value)
                        jobcls.Job_Skills = dr["Skills"].ToString();

                    // Read Salary
                    if (dr.HasColumn("Job_Salary") && dr["Job_Salary"] != DBNull.Value)
                        jobcls.Job_Salary = dr["Job_Salary"].ToString();
                    else
                        jobcls.Job_Salary = "Competitive";

                    // Read End Date
                    if (dr.HasColumn("Job_enddate") && dr["Job_enddate"] != DBNull.Value)
                        jobcls.Job_enddate = Convert.ToDateTime(dr["Job_enddate"]);
                    else if (dr.HasColumn("EndDate") && dr["EndDate"] != DBNull.Value)
                        jobcls.Job_enddate = Convert.ToDateTime(dr["EndDate"]);
                    else
                        jobcls.Job_enddate = DateTime.Today.AddDays(30);

                    // Read Location
                    if (dr.HasColumn("Job_Location") && dr["Job_Location"] != DBNull.Value)
                        jobcls.Job_Location = dr["Job_Location"].ToString();
                    else if (dr.HasColumn("Location") && dr["Location"] != DBNull.Value)
                        jobcls.Job_Location = dr["Location"].ToString();

                    joblist.selectjobdetails.Add(jobcls);
                }
                con.Close();
                return joblist;
            }
        }
    }
}
