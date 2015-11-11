using System.Web.Mvc;

namespace DocumentFlow.Controllers
{
    public class AdminController : Controller
    {
        // GET
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Admin");
        }
    }
}