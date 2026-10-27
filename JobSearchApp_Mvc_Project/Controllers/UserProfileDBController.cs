using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchApp_Mvc_Project.Models;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class UserProfileDBController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();
        private object obj;

        // GET: UserProfileDB
        public ActionResult Profile_Load()
        {
            int id = Convert.ToInt32(Session["uid"]);
            var getdata = dbobj.sp_profile(id).FirstOrDefault();

            return View(new ProfileCls
            {
                name = getdata.Name,
                age = getdata.Age,
                address = getdata.Address,
                email = getdata.Email,
                photo = getdata.Photo,

            });
        }

        [HttpPost]
        public ActionResult Profile_Update(ProfileCls obj, HttpPostedFileBase file)
        {
            int id = Convert.ToInt32(Session["uid"]);

            string photoName = "";
            if (file != null && file.ContentLength > 0)
            {
                string fname = Path.GetFileName(file.FileName);
                string path = Path.Combine(Server.MapPath("~/Photos/"), fname);
                file.SaveAs(path);
                photoName = fname;
            }

            dbobj.sp_profile_Update(id, obj.age, obj.address, photoName);
            var getdata = dbobj.sp_profile(id).FirstOrDefault();

            return View("Profile_Load", new ProfileCls
            {
                name = getdata.Name,
                age = getdata.Age,
                address = getdata.Address,
                email = getdata.Email,
                photo = getdata.Photo,
                msg = "Profile updated"
            });
        }

        [HttpPost]
        public ActionResult Profile_Load(ProfileCls obj, HttpPostedFileBase file)
        {
            return Profile_Update(obj, file);
        }
    }
}
