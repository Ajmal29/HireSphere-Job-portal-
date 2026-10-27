using JobSearchApp_Mvc_Project.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class UserRegController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();

        public ActionResult Insertuser_Pageload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insertuser_click(UserInsert clsobj, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                var getmaxid = dbobj.sp_Maxidlogin().FirstOrDefault();

                int mid = Convert.ToInt32(getmaxid);
                int regid = 0;

                if (mid == 0)
                {
                    regid = 1;
                }
                else
                {
                    regid = mid + 1;
                }

                // File Upload for Photo
                if (file != null && file.ContentLength > 0)
                {
                    string fname = Path.GetFileName(file.FileName);
                    string path = Path.Combine(Server.MapPath("~/Photos/"), fname);
                    file.SaveAs(path);
                    clsobj.photo = "~/Photos/" + fname;
                }
                else
                {
                    clsobj.photo = "";
                }

                clsobj.status = 1;

                dbobj.sp_Userreg(
                    regid,
                    clsobj.name,
                    clsobj.age,
                    clsobj.email,
                    clsobj.address,
                    clsobj.phone,
                    clsobj.photo,
                    clsobj.qualification,
                    Convert.ToInt32(clsobj.experience),
                    clsobj.skills,
                    clsobj.status.ToString()
                );


                dbobj.sp_Loginsert(
                    regid,
                    clsobj.username,
                    clsobj.pass,
                    "User" // Or "Employee"
                );

                clsobj.usermsg = "successfully inserted";

                return View("Insertuser_Pageload", clsobj);
            }

            return View("Insertuser_Pageload", clsobj);
        }
    }
}