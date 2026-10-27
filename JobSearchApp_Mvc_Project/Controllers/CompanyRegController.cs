using JobSearchApp_Mvc_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class CompanyRegController : Controller
    {
        // GET: CompanyReg
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();

        public ActionResult Insertcompany_Pageload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Insertcompany_click(CompanyInsert clsobj)
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

                dbobj.sp_Companyreg(
                    clsobj.name,
                    clsobj.address,
                    clsobj.phone,
                    clsobj.email
                );


                dbobj.sp_Loginsert(
                    regid,
                    clsobj.username,
                    clsobj.pass,
                    "Employer"
                );

                clsobj.companymsg = "successfully inserted";

                return View("Insertcompany_Pageload", clsobj);
            }

            return View("Insertcompany_Pageload", clsobj);
        }
    }
}