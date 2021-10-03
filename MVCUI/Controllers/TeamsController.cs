using MVCUI.Models;
using System.Linq;
using System.Web.Mvc;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace MVCUI.Controllers
{
    public class TeamsController : Controller
    {
        public ActionResult Index()
        {
            var teams = GlobalConfig.Connection.GetAllTeams();
            return View(teams);
        }

        public ActionResult Create()
        {
            TeamMVCModel input = new TeamMVCModel();
            var people = GlobalConfig.Connection.GetAllPeople();

            input.TeamMembers = people.Select(p => new SelectListItem()
            {
                Text = p.FullName,
                Value = p.Id.ToString()
            }).ToList();

            return View(input);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TeamMVCModel teamMVC)
        {
            try
            {
                if (teamMVC.SelectedTeamMembers.Count <= 0)
                {
                    ModelState.AddModelError(nameof(teamMVC.SelectedTeamMembers.Count), "Invalid selected team members count");
                }

                if (ModelState.IsValid)
                {
                    TeamModel team = new TeamModel()
                    {
                        TeamName = teamMVC.TeamName,
                        TeamMembers = teamMVC.SelectedTeamMembers.Select(t => new PersonModel { Id = int.Parse(t) }).ToList()
                    };

                    GlobalConfig.Connection.CreateTeam(team);

                    return RedirectToAction(nameof(Index));
                }

                TeamMVCModel input = new TeamMVCModel();
                var people = GlobalConfig.Connection.GetAllPeople();

                input.TeamMembers = people.Select(p => new SelectListItem()
                {
                    Text = p.FullName,
                    Value = p.Id.ToString()
                }).ToList();
                return View(input);
            }
            catch
            {
                return View();
            }
        }
    }
}
