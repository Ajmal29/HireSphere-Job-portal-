using System.Web.Mvc;

namespace JobSearchApp_Mvc_Project.Controllers
{
    public class LoginDBController : Controller
    {
        public ActionResult Home()
        {
            return RedirectToAction("UserHome", "UserHome");
        }
    }
}
