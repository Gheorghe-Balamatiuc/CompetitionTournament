using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Competition_Tournament.Data;
using Competition_Tournament.Models;

namespace Competition_Tournament.Controllers
{
    public class GamesController : Controller
    {
        private readonly CompetitionManagementContext _context;

        public GamesController(CompetitionManagementContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index(int? competitionId)
        {
            IQueryable<Game> games = _context.Games.Include(g => g.Competition).Include(g => g.Team1).Include(g => g.Team2);

            if (competitionId != null)
            {
                games = games.Where(g => g.CompetitionId == competitionId);
            }

            ViewData["Competitions"] = new SelectList(_context.Competitions, "Id", "Name");

            return View(await games.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .Include(g => g.Team1)
                .Include(g => g.Team2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create(int? id)
        {
            var competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(g => g.Id == id);
            if(competition == null) { return NotFound(); }
            ViewData["id"] = id;
            ViewBag.CompetitionName = _context.Competitions.FirstOrDefault(c => c.Id == id)?.Name;
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Name");
            ViewData["Team1Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == id)), "Id", "Name");
            ViewData["Team2Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == id)), "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Team1Id,Team2Id,Team1Goals,Team2Goals,Team1Name,Team2Name,CompetitionId,Date,Stadium")] Game game, int CompId)
        {
            var competition = _context.Competitions.Include(c => c.CompetitionTypeNavigation).Include(c => c.Teams).FirstOrDefault(g => g.Id == CompId);
            if (competition.CompetitionTypeNavigation.Name == "Knockout")
            {
                if (game.Team1Id == game.Team2Id)
                {
                    ModelState.AddModelError(string.Empty, "You cannot add a game between two similar teams.");
                }
                else
                {
                    var existingGame = _context.Games.FirstOrDefault(g => g.CompetitionId == CompId && ((g.Team1Id == game.Team1Id && g.Team2Id == game.Team2Id) || (g.Team1Id == game.Team2Id && g.Team2Id == game.Team1Id)));
                    if (existingGame != null)
                    {
                        ModelState.AddModelError(string.Empty, "A game between these two teams already exists in this knockout competition.");
                    }
                }
            }

            if (ModelState.IsValid)
            {
                game.Team1Name = _context.Teams.Find(game.Team1Id).Name;
                game.Team2Name = _context.Teams.Find(game.Team2Id).Name;
                game.CompetitionId = CompId;
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == CompId)), "Id", "Name");
            ViewData["Team2Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == CompId)), "Id", "Name");
            ViewBag.id = CompId;
            return View(game);
        }

        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            var competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(g => g.Id == id);
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == id)), "Id", "Name");
            ViewData["Team2Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == id)), "Id", "Name");
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Team1Id,Team2Id,Team1Goals,Team2Goals,CompetitionId,Date,Stadium,Team1Name,Team2Name")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    game.Team1Name = _context.Teams.Find(game.Team1Id).Name;
                    game.Team2Name = _context.Teams.Find(game.Team2Id).Name;
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
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
            var competition = _context.Competitions.Include(c => c.Teams).FirstOrDefault(g => g.Id == id);
            ViewData["CompetitionId"] = new SelectList(_context.Competitions, "Id", "Id", game.CompetitionId);
            ViewData["Team1Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == id)), "Id", "Name");
            ViewData["Team2Id"] = new SelectList(_context.Teams.Where(t => t.Competitions.Any(c => c.Id == id)), "Id", "Name");
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Games == null)
            {
                return NotFound();
            }

            var game = await _context.Games
                .Include(g => g.Competition)
                .Include(g => g.Team1)
                .Include(g => g.Team2)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Games == null)
            {
                return Problem("Entity set 'CompetitionManagementContext.Games'  is null.");
            }
            var game = await _context.Games.FindAsync(id);
            if (game != null)
            {
                _context.Games.Remove(game);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
          return (_context.Games?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
