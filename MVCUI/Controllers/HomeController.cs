using System.Web.Mvc;
using TrackerLibrary;

namespace MVCUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var tournaments = GlobalConfig.Connection.GetAllTournaments();
            return View(tournaments);
        }
    }
}