using System;
using System.Linq;
using System.Web.Mvc;
using JobSearchApp_Mvc_Project.Models;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class ChangePwdController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();
        // private object obj;

        // GET: ChangePwd
        public ActionResult pwd_Load()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            int id = Convert.ToInt32(Session["uid"]);
            string uname = Session["uname"] != null ? Session["uname"].ToString() : "";
            var getdata = dbobj.sp_selectPwd(id, uname).FirstOrDefault();

            return View(new ChangePwdCls
            {
                OldPassword = getdata
            });
        }
        
        [HttpPost]
        public ActionResult pwd_Click(ChangePwdCls obj)
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            if (ModelState.IsValid)
            {
                int id = Convert.ToInt32(Session["uid"]);
                string uname = Session["uname"] != null ? Session["uname"].ToString() : "";
                dbobj.sp_pwdchange(id, (string)obj.NewPassword, uname);

                obj.msg = "password changed";
                obj.OldPassword = obj.NewPassword;

                return View("pwd_Load", obj);
            }

            return View("pwd_Load", obj);
        }
    }

    public class ChangePwdDBController : ChangePwdController
    {
    }
}

