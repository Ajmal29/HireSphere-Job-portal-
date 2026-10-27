using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class HomeController : Controller
    {
      
            public ActionResult Index()
            {
                return View();
            }

            public ActionResult About()
            {
                ViewBag.Message = "HireSphere is a modern portal connecting jobseekers and top companies.";
                return View();
            }

            public ActionResult Contact()
            {
                ViewBag.Message = "Feel free to reach out to our team.";
                return View();
            }
        }
    }

