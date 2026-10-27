using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchApp_Mvc_Project.Models;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class CompanyHomeController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();

        // GET: CompanyHome
        public ActionResult CompanyHome()
        {
            if (Session["uid"] == null || Session["utype"] == null || !Session["utype"].ToString().Equals("Employer", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            int cid = Convert.ToInt32(Session["uid"]);
            ViewBag.CompanyName = Session["uname"];
            ViewBag.Msg = TempData["msg"];

            // Automatically expire past jobs where EndDate < Current Date
            try
            {
                DateTime today = DateTime.Today;
                dbobj.Database.ExecuteSqlCommand("UPDATE Job SET JobStatus = 'Expired' WHERE EndDate IS NOT NULL AND CAST(EndDate AS DATE) < {0} AND (JobStatus IS NULL OR JobStatus <> 'Expired')", today);
            }
            catch { }

            // Fetch jobs posted by this company
            try
            {
                var allJobs = dbobj.sp_Selectall().Where(j => j.CompanyId == cid).ToList();
                ViewBag.CompanyJobs = allJobs;
            }
            catch
            {
                ViewBag.CompanyJobs = null;
            }

            return View();
        }

        // GET: CompanyHome/UpdateExpiredJobs
        public ActionResult UpdateExpiredJobs()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            DateTime today = DateTime.Today;
            int count = 0;
            try
            {
                count = dbobj.Database.ExecuteSqlCommand("UPDATE Job SET JobStatus = 'Expired' WHERE EndDate IS NOT NULL AND CAST(EndDate AS DATE) < {0} AND (JobStatus IS NULL OR JobStatus <> 'Expired')", today);
                TempData["msg"] = $"Checked against date ({today:yyyy-MM-dd}). {count} expired job(s) updated to 'Expired' status.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error updating jobs: " + ex.Message;
            }

            return RedirectToAction("CompanyHome");
        }

        // GET: CompanyHome/ExpireJob
        public ActionResult ExpireJob(int id)
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            int cid = Convert.ToInt32(Session["uid"]);
            try
            {
                dbobj.Database.ExecuteSqlCommand("UPDATE Job SET JobStatus = 'Expired' WHERE JobId = {0} AND CompanyId = {1}", id, cid);
                TempData["msg"] = "Job status updated to Expired.";
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error: " + ex.Message;
            }

            return RedirectToAction("CompanyHome");
        }

        // GET: CompanyHome/AddJob
        public ActionResult AddJob()
        {
            if (Session["uid"] == null || Session["utype"] == null || !Session["utype"].ToString().Equals("Employer", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            return View(new JobInsert());
        }

        // POST: CompanyHome/AddJob
        [HttpPost]
        public ActionResult AddJob(JobInsert clsobj)
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            if (ModelState.IsValid)
            {
                int cid = Convert.ToInt32(Session["uid"]);
                string status = string.IsNullOrEmpty(clsobj.JobStatus) ? "Active" : clsobj.JobStatus;

                // Call sp_Jobinsert(@cid, @jbt, @jbexp, @jskill, @jqual, @jloc, @sdate, @edate, @jsta)
                dbobj.sp_Jobinsert(
                    cid,
                    clsobj.JobTitle,
                    clsobj.ExperienceYears,
                    clsobj.Skills,
                    clsobj.Qualification,
                    clsobj.Location,
                    clsobj.StartDate,
                    clsobj.EndDate,
                    status
                );

                clsobj.msg = "Job details inserted successfully!";
                ModelState.Clear();
                return View(new JobInsert { msg = "Job details inserted successfully!" });
            }

            return View(clsobj);
        }
    }
}

