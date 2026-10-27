using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using JobSearchApp_Mvc_Project.Models;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class JobInsertController : Controller
    {
        MVCProjectJobSearchEntities dbobj = new MVCProjectJobSearchEntities();

        // GET: JobInsert
        public ActionResult Insert_Pageload()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login_Pageload", "Login");
            }

            // Automatically expire past jobs on page load
            try
            {
                DateTime today = DateTime.Today;
                dbobj.Database.ExecuteSqlCommand("UPDATE Job SET JobStatus = 'Expired' WHERE EndDate IS NOT NULL AND CAST(EndDate AS DATE) " +
                    "< {0} AND (JobStatus IS NULL OR JobStatus <> 'Expired')", today);
            }
            catch { }

            JobInsert obj = new JobInsert();
            obj.MyFavouriteQual = getQualificationData();
            obj.MyFavouriteSkill = GetSkillData();
            return View(obj);
        }

        public List<CheckBoxListHelper> getQualificationData()
        {
            List<CheckBoxListHelper> sts = new List<CheckBoxListHelper>()
            {
                new CheckBoxListHelper { Value = "1001", Text = "SSLC", IsChecked = false },
                new CheckBoxListHelper { Value = "1002", Text = "PLUS TWO", IsChecked = false },
                new CheckBoxListHelper { Value = "1003", Text = "BCA", IsChecked = false },
                new CheckBoxListHelper { Value = "1004", Text = "B.TECH", IsChecked = false },
                new CheckBoxListHelper { Value = "1005", Text = "M.TECH", IsChecked = false }
            };
            return sts;
        }

        public List<CheckBoxListHelper> GetSkillData()
        {
            List<CheckBoxListHelper> sts = new List<CheckBoxListHelper>()
            {
                new CheckBoxListHelper { Value = "10001", Text = "C#", IsChecked = false },
                new CheckBoxListHelper { Value = "10002", Text = "JAVA", IsChecked = false },
                new CheckBoxListHelper { Value = "10003", Text = "ASP.NET MVC", IsChecked = false },
                new CheckBoxListHelper { Value = "10004", Text = "PYTHON", IsChecked = false },
                new CheckBoxListHelper { Value = "10005", Text = "DJANGO", IsChecked = false }
            };
            return sts;
        }

        [HttpPost]
        public ActionResult Insert_click(JobInsert clsobj, FormCollection form)
        {
            if (ModelState.IsValid)
            {
                var qual = clsobj.selectedQual != null ? string.Join(",", clsobj.selectedQual) : "";
                clsobj.Qualification = qual;
                clsobj.MyFavouriteQual = getQualificationData();

                // Combine typed skills with selected checkbox skills
                var checkedSkills = clsobj.selectedSkill != null ? string.Join(",", clsobj.selectedSkill) : "";
                if (!string.IsNullOrWhiteSpace(clsobj.Skills) && !string.IsNullOrWhiteSpace(checkedSkills))
                {
                    clsobj.Skills = clsobj.Skills.Trim() + ", " + checkedSkills;
                }
                else if (!string.IsNullOrWhiteSpace(checkedSkills))
                {
                    clsobj.Skills = checkedSkills;
                }
                clsobj.MyFavouriteSkill = GetSkillData();

                var cid = Convert.ToInt32(Session["uid"]);

                // Calling sp_JobInsert
                dbobj.sp_Jobinsert(
                    cid, 
                    clsobj.JobTitle, 
                    Convert.ToInt32(clsobj.JobExperience), 
                    clsobj.Skills, 
                    clsobj.Qualification, 
                    clsobj.JobLocation, 
                    Convert.ToDateTime(clsobj.StartDate), 
                    Convert.ToDateTime(clsobj.EndDate), 
                    "Active"
                );

                clsobj.msg = "Successfully inserted";
                return View("Insert_Pageload", clsobj);
            }

            clsobj.MyFavouriteQual = getQualificationData();
            clsobj.MyFavouriteSkill = GetSkillData();
            return View("Insert_Pageload", clsobj);
        }
    }
}

