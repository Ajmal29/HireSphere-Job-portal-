using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchApp_Mvc_Project.Models;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class LoginController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();

        public ActionResult Login_Pageload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login_click(LoginClass clsobj)
        {
            if (ModelState.IsValid)
            {

                var count = dbobj.sp_Logincountid(clsobj.username, clsobj.password).FirstOrDefault();

                if (Convert.ToInt32(count) == 1)
                {

                    var logid = dbobj.sp_Loginid(clsobj.username, clsobj.password).FirstOrDefault();

                    var logtype = dbobj.sp_Logtype(clsobj.username, clsobj.password).FirstOrDefault();

                    Session["uid"] = Convert.ToInt32(logid);
                    Session["uname"] = clsobj.username;
                    Session["utype"] = logtype.ToString();


                    if (logtype.ToString().Equals("Employer", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("Insert_Pageload", "JobInsert");
                    }
                    else if (logtype.ToString().Equals("User", StringComparison.OrdinalIgnoreCase) || 
                             logtype.ToString().Equals("Employee", StringComparison.OrdinalIgnoreCase))
                    {
                        return RedirectToAction("jobview_pageload", "JobView");
                    }
                }
                else
                {
                    clsobj.msg = "Invalid Username or Password";
                    return View("Login_Pageload", clsobj);
                }
            }

            return View("Login_Pageload", clsobj);
        }
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login_Pageload", "Login");
        }
    }
}

