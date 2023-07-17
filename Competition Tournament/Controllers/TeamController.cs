using Competition_Tournament.Data;
using Competition_Tournament.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Competition_Tournament.Controllers
{
    public class TeamController : Controller
    {
        private readonly CompetitionManagementContext _context; //var privata pentru clasa asta

        public TeamController(CompetitionManagementContext context) //constructorul
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? competitionId)
        {
            var teams = _context.Teams.AsQueryable();

            if (competitionId != null)
            {
                teams = teams.Where(t => t.Competitions.Any(c => c.Id == competitionId));
            }

            ViewData["Competitions"] = new SelectList(_context.Competitions, "Id", "Name");
            ViewBag.SelectedCompetitionId = competitionId;

            return View(await teams.ToListAsync());
        }

        public IActionResult Create() 
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        [HttpPost]
        public IActionResult Create(Team team)
        {
            if (team.CreatedOn > DateTime.Today)
            {
                ModelState.AddModelError("CreatedOn", "The date cannot be in the future.");
            }

            if (_context.Teams.Any(t => t.Name == team.Name))
            {
                ModelState.AddModelError("Name", "A team with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Teams.Add(team);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                var players = _context.Players.Where(p => p.IdTeam == id);
                foreach (var player in players)
                {
                    // To delete the player:
                    // _context.Players.Remove(player);

                    // To disassociate the player from the team:
                    player.IdTeam = null;
                }

                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Edit(Team team)
        {
            if (team.CreatedOn > DateTime.Today)
            {
                ModelState.AddModelError("CreatedOn", "The date cannot be in the future.");
            }

            if (_context.Teams.Any(t => t.Name == team.Name && t.Id != team.Id))
            {
                ModelState.AddModelError("Name", "A team with this name already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Entry(team).State = EntityState.Modified;
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }
    }
}