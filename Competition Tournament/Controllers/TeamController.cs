using Competition_Tournament.Data;
using Competition_Tournament.Models;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public IActionResult Create(Team team)
        {
            _context.Teams.Add(team);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
