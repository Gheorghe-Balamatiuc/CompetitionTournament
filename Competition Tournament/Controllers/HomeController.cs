using Competition_Tournament.Data;
using Competition_Tournament.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Competition_Tournament.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CompetitionManagementContext _context;

        public HomeController(ILogger<HomeController> logger, CompetitionManagementContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var competitions = await _context.Competitions.Include(c => c.CompetitionTypeNavigation).ToListAsync();

            var competitionWinners = new Dictionary<int, string>();

            foreach (var competition in competitions)
            {
                if (competition.CompetitionTypeNavigation.Name == "Knockout")
                {
                    var games = await _context.Games.Where(g => g.CompetitionId == competition.Id).ToListAsync();

                    var teamWins = new Dictionary<int, int>();
                    var teamLosses = new Dictionary<int, int>();

                    foreach (var game in games)
                    {
                        if (game.Team1Goals != null && game.Team2Goals != null)
                        {
                            if (game.Team1Id != null && game.Team2Id != null)
                            {
                                if (!teamWins.ContainsKey(game.Team1Id.Value))
                                {
                                    teamWins[game.Team1Id.Value] = 0;
                                    teamLosses[game.Team1Id.Value] = 0;
                                }
                                if (!teamWins.ContainsKey(game.Team2Id.Value))
                                {
                                    teamWins[game.Team2Id.Value] = 0;
                                    teamLosses[game.Team2Id.Value] = 0;
                                }

                                if (game.Team1Goals > game.Team2Goals)
                                {
                                    teamWins[game.Team1Id.Value] += 1;
                                    teamLosses[game.Team2Id.Value] += 1;
                                }
                                else
                                {
                                    teamWins[game.Team2Id.Value] += 1;
                                    teamLosses[game.Team1Id.Value] += 1;
                                }
                            }
                        }
                    }

                    var teamsWithZeroLosses = teamLosses.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key).ToList();
                    if (teamsWithZeroLosses.Count == 0)
                    {
                        competitionWinners[competition.Id] = "No teams with 0 losses";
                    }
                    else if (teamsWithZeroLosses.Count == 1)
                    {
                        var winnerTeam = await _context.Teams.FindAsync(teamsWithZeroLosses[0]);
                        competitionWinners[competition.Id] = winnerTeam.Name;
                    }
                    else
                    {
                        competitionWinners[competition.Id] = "Multiple teams with 0 losses";
                    }
                }
                else if (competition.CompetitionTypeNavigation.Name == "Round-robin")
                {
                    var games = await _context.Games.Where(g => g.CompetitionId == competition.Id).ToListAsync();

                    var teamPoints = new Dictionary<int, int>();

                    foreach (var game in games)
                    {
                        if (game.Team1Goals != null && game.Team2Goals != null)
                        {
                            if (game.Team1Id != null && game.Team2Id != null)
                            {
                                if (!teamPoints.ContainsKey(game.Team1Id.Value))
                                {
                                    teamPoints[game.Team1Id.Value] = 0;
                                }
                                if (!teamPoints.ContainsKey(game.Team2Id.Value))
                                {
                                    teamPoints[game.Team2Id.Value] = 0;
                                }

                                if (game.Team1Goals == game.Team2Goals)
                                {
                                    teamPoints[game.Team1Id.Value] += 1;
                                    teamPoints[game.Team2Id.Value] += 1;
                                }
                                else if (game.Team1Goals > game.Team2Goals)
                                {
                                    teamPoints[game.Team1Id.Value] += 3;
                                }
                                else
                                {
                                    teamPoints[game.Team2Id.Value] += 3;
                                }
                            }
                        }
                    }

                    if (teamPoints.Count == 0)
                    {
                        competitionWinners[competition.Id] = "No teams with points";
                    }
                    else
                    {
                        int maxPoints = teamPoints.Values.Max();

                        var teamsWithMaxPoints = teamPoints.Where(kvp => kvp.Value == maxPoints).Select(kvp => kvp.Key).ToList();
                        if (teamsWithMaxPoints.Count == 1)
                        {
                            var winnerTeam = await _context.Teams.FindAsync(teamsWithMaxPoints[0]);
                            competitionWinners[competition.Id] = winnerTeam.Name;
                        }
                        else
                        {
                            competitionWinners[competition.Id] = "Multiple teams with the maximum number of points";
                        }
                    }
                }
            }

            ViewBag.CompetitionWinners = competitionWinners;

            return View(competitions);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
