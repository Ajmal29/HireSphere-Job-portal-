using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class DisplayAllController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();

        // GET: DisplayAll
        public ActionResult Display_Pageload()
        {
            var data = dbobj.sp_Selectall().ToList();
            ViewBag.userdetails = data;
            ViewBag.jobdetails = data;
            return View();
        }
    }
}