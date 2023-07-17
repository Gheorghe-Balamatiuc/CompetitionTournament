using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Competition_Tournament.Data;
using Competition_Tournament.Models;
using Competition_Tournament.Models.ViewModel;

namespace Competition_Tournament.Controllers
{
    public class CompetitionsController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public CompetitionsController(CompetitionManagementContext context)
        {
            _context = context;
        }

        // GET: Competitions
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        // GET: Competitions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionTypeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // GET: Competitions/Create
        public IActionResult Create()
        {
            ViewData["CompetitionType"] = new SelectList(_context.CompetitionTypes, "Id", "Name");
            return View();
        }

        // POST: Competitions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,Location,CompetitionType")] Competition competition)
        {
            if (_context.Competitions.Any(c => c.Name == competition.Name))
            {
                ModelState.AddModelError("Name", "A competition with this name already exists.");
            }
            if(competition.StartDate >= competition.EndDate)
            {
                ModelState.AddModelError(string.Empty, "The end date should be grater that the start date.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(competition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompetitionType"] = new SelectList(_context.CompetitionTypes, "Id", "Name", competition.CompetitionType);
            return View(competition);
        }

        // GET: Competitions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FindAsync(id);
            var teams = await _context.Teams.ToListAsync();

            if (competition == null)
            {
                return NotFound();
            }
            ViewData["CompetitionType"] = new SelectList(_context.CompetitionTypes, "Id", "Name", competition.CompetitionType);
            ViewData["Teams"] = new SelectList(teams, "Id", "Name");
            
            return View(competition);
        }

        // POST: Competitions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,Location,CompetitionType")] Competition competition)
        {
            var competitionInDb = _context.Competitions.AsNoTracking().FirstOrDefault(c => c.Name == competition.Name);
            if (competitionInDb != null && competitionInDb.Id != competition.Id)
            {
                ModelState.AddModelError("Name", "The competition name already exists.");
            }
            if (competition.StartDate >= competition.EndDate)
            {
                ModelState.AddModelError(string.Empty, "The end date should be grater that the start date.");
            }
            if (ModelState.IsValid)
                if (id != competition.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competition);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionExists(competition.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompetitionType"] = new SelectList(_context.CompetitionTypes, "Id", "Name", competition.CompetitionType);
            return View(competition);
        }

        // GET: Competitions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions
                .Include(c => c.CompetitionTypeNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (competition == null)
            {
                return NotFound();
            }

            return View(competition);
        }

        // POST: Competitions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Competitions == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Competitions'  is null.");
            }
            var competition = await _context.Competitions.FindAsync(id);
            if (competition != null)
            {
                _context.Competitions.Remove(competition);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompetitionExists(int id)
        {
          return (_context.Competitions?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> AddTeam(int id)
        {
            var competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(m => m.Id == id);
            ICollection<Team> addedTeams = competition.Teams;
            var availableTeams = _context.Teams.Where(t => !addedTeams.Contains(t));

            TeamSelector teamSelector = new TeamSelector();
            teamSelector.Competition = competition;

            if (competition == null)
            {
                return NotFound();
            }

            ViewBag.id = competition.Id;
            ViewData["availableTeams"] = new SelectList(availableTeams, "Id", "Name");
            ViewData["addedTeams"] = new SelectList(addedTeams, "Id", "Name");

            return View(teamSelector);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTeamConfirmed(int id, TeamSelector teamSelector)
        {
            var competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(m => m.Id == id);
            ICollection<Team> addedTeams = competition.Teams;
            var availableTeams = _context.Teams.Where(t => !addedTeams.Contains(t));

            if (ModelState.IsValid)
            {
                var team = await _context.Teams.FindAsync(teamSelector.Team);
                if (team != null)
                {
                    competition.Teams.Add(team);
                    await _context.SaveChangesAsync();
                }
            }

            ViewBag.id = competition.Id;
            ViewData["availableTeams"] = new SelectList(availableTeams, "Id", "Name");
            ViewData["addedTeams"] = new SelectList(addedTeams, "Id", "Name");

            return RedirectToAction(nameof(AddTeam), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTeam(int id, int teamId)
        {
            var competition = await _context.Competitions
                .Include(c => c.Teams)
                .FirstOrDefaultAsync(m => m.Id == id);
            ICollection<Team> addedTeams = competition.Teams;
            var availableTeams = _context.Teams.Where(t => !addedTeams.Contains(t));

            var team = await _context.Teams.FindAsync(teamId);
            if (team != null)
            {
                competition.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }

            ViewBag.id = competition.Id;
            ViewData["availableTeams"] = new SelectList(availableTeams, "Id", "Name");
            ViewData["addedTeams"] = new SelectList(addedTeams, "Id", "Name");

            return RedirectToAction(nameof(AddTeam), new { id });
        }

        public async Task<IActionResult> Leaderboard(int? id)
        {
            var teams = _context.Teams.AsQueryable();

            if (id != null)
            {
                teams = teams.Where(t => t.Competitions.Any(c => c.Id == id));
            }

            ViewData["Competitions"] = new SelectList(_context.Competitions, "Id", "Name");

            var games = await _context.Games.Where(g => g.CompetitionId == id).ToListAsync();

            var teamPoints = new Dictionary<int, int>();
            var teamGoalsScored = new Dictionary<int, int>();
            var teamGoalsConceded = new Dictionary<int, int>();

            foreach (var game in games)
            {
                // Check if the goals are not null
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

                        if (!teamGoalsScored.ContainsKey(game.Team1Id.Value))
                        {
                            teamGoalsScored[game.Team1Id.Value] = 0;
                        }
                        if (!teamGoalsScored.ContainsKey(game.Team2Id.Value))
                        {
                            teamGoalsScored[game.Team2Id.Value] = 0;
                        }

                        if (!teamGoalsConceded.ContainsKey(game.Team1Id.Value))
                        {
                            teamGoalsConceded[game.Team1Id.Value] = 0;
                        }
                        if (!teamGoalsConceded.ContainsKey(game.Team2Id.Value))
                        {
                            teamGoalsConceded[game.Team2Id.Value] = 0;
                        }

                        teamGoalsScored[game.Team1Id.Value] += game.Team1Goals.Value;
                        teamGoalsScored[game.Team2Id.Value] += game.Team2Goals.Value;

                        teamGoalsConceded[game.Team1Id.Value] += game.Team2Goals.Value;
                        teamGoalsConceded[game.Team2Id.Value] += game.Team1Goals.Value;

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

            var participatingTeams = await teams.ToListAsync();
            participatingTeams = participatingTeams.Where(t => teamPoints.ContainsKey(t.Id)).ToList();

            var sortedTeams = participatingTeams.OrderByDescending(t => teamPoints[t.Id]).ToList();

            ViewBag.TeamPoints = teamPoints;
            ViewBag.id = id;
            ViewBag.GoalsScored = teamGoalsScored;
            ViewBag.GoalsConceded = teamGoalsConceded;

            return View(sortedTeams);
        }

        public async Task<IActionResult> LeaderboardKnockout(int? id)
        {
            var teams = _context.Teams.AsQueryable();

            if (id != null)
            {
                teams = teams.Where(t => t.Competitions.Any(c => c.Id == id));
            }

            ViewData["Competitions"] = new SelectList(_context.Competitions, "Id", "Name");

            var games = await _context.Games.Where(g => g.CompetitionId == id).ToListAsync();

            var teamWins = new Dictionary<int, int>();
            var teamLosses = new Dictionary<int, int>();
            var teamGoalsScored = new Dictionary<int, int>();
            var teamGoalsConceded = new Dictionary<int, int>();

            foreach (var game in games)
            {
                // Check if the goals are not null
                if (game.Team1Goals != null && game.Team2Goals != null)
                {
                    if (game.Team1Id != null && game.Team2Id != null)
                    {
                        if (!teamWins.ContainsKey(game.Team1Id.Value))
                        {
                            teamWins[game.Team1Id.Value] = 0;
                            teamLosses[game.Team1Id.Value] = 0;
                            teamGoalsScored[game.Team1Id.Value] = 0;
                            teamGoalsConceded[game.Team1Id.Value] = 0;
                        }
                        if (!teamWins.ContainsKey(game.Team2Id.Value))
                        {
                            teamWins[game.Team2Id.Value] = 0;
                            teamLosses[game.Team2Id.Value] = 0;
                            teamGoalsScored[game.Team2Id.Value] = 0;
                            teamGoalsConceded[game.Team2Id.Value] = 0;
                        }

                        teamGoalsScored[game.Team1Id.Value] += game.Team1Goals.Value;
                        teamGoalsScored[game.Team2Id.Value] += game.Team2Goals.Value;

                        teamGoalsConceded[game.Team1Id.Value] += game.Team2Goals.Value;
                        teamGoalsConceded[game.Team2Id.Value] += game.Team1Goals.Value;

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

            var participatingTeams = await teams.ToListAsync();
            participatingTeams = participatingTeams.Where(t => teamWins.ContainsKey(t.Id)).ToList();

            var sortedTeams = participatingTeams.OrderBy(t => teamLosses[t.Id]).ToList();

            ViewBag.TeamWins = teamWins;
            ViewBag.TeamLosses = teamLosses;
            ViewBag.id = id;
            ViewBag.GoalsScored = teamGoalsScored;
            ViewBag.GoalsConceded = teamGoalsConceded;

            return View(sortedTeams);
        }
    }
}
