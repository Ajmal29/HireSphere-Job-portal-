using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class UserHomeController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();

        // GET: UserHome
        public ActionResult UserHome()
        {
            if (Session["uid"] == null || Session["utype"] == null)
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            ViewBag.UserName = Session["uname"];

            // Fetch all jobs posted by companies using sp_Selectall
            var jobs = dbobj.sp_Selectall().ToList();
            ViewBag.jobdetails = jobs;

            return View();
        }

        // GET: UserHome/DisplayJobs
        public ActionResult DisplayJobs()
        {
            return RedirectToAction("UserHome");
        }
    }
}
