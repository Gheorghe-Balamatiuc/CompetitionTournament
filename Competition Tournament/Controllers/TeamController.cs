using Competition_Tournament.Data;
using Competition_Tournament.Models;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            return View(_context.Teams.ToList());
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

            if (ModelState.IsValid)
            {
                _context.Teams.Add(team);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(team);
        }

        public IActionResult Delete(int id)
        {
            var team = _context.Teams.Find(id);
            if (team == null)
            {
                return NotFound();
            }
            _context.Teams.Remove(team);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(Team team)
        {
            if (team.CreatedOn > DateTime.Today)
            {
                ModelState.AddModelError("CreatedOn", "The date cannot be in the future.");
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