using MVCUI.Models;
using System.Linq;
using System.Web.Mvc;
using TournamentTracker;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace MVCUI.Controllers
{
    public class TournamentsController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Create()
        {
            var teams = GlobalConfig.Connection.GetAllTeams();
            var prizes = GlobalConfig.Connection.GetAllPrizes();

            TournamentMVCModel tournamentMVC = new TournamentMVCModel()
            {
                EnteredTeams = teams.Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.TeamName
                }).ToList(),

                Prizes = prizes.Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = p.PlaceName
                }).ToList()
            };

            return View(tournamentMVC);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TournamentMVCModel tournamentMVC)
        {
            try
            {
                if (tournamentMVC.SelectedEnteredTeams.Count <= 0)
                {
                    ModelState.AddModelError(nameof(tournamentMVC.SelectedEnteredTeams.Count), "Invalid selected teams count");
                }

                if (tournamentMVC.SelectedPrizes.Count <= 0)
                {
                    ModelState.AddModelError(nameof(tournamentMVC.SelectedPrizes.Count), "Invalid selected prizes count");
                }

                if (ModelState.IsValid)
                {
                    TournamentModel tournament = new TournamentModel()
                    {
                        TournamentName = tournamentMVC.TournamentName,
                        EntryFee = tournamentMVC.EntryFee,
                        EnteredTeams = tournamentMVC.SelectedEnteredTeams.Select(e => new TeamModel { Id = int.Parse(e) }).ToList(),
                        Prizes = tournamentMVC.SelectedPrizes.Select(p => new PrizeModel { Id = int.Parse(p) }).ToList()
                    };

                    TournamentLogic.CreateRounds(tournament);

                    GlobalConfig.Connection.CreateTournament(tournament);

                    tournament.AlertUsersToNewRound();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }
    }
}