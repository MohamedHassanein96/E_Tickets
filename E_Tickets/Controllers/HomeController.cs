using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace E_Tickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly ILogger<HomeController> _logger;
        

        public HomeController(ILogger<HomeController> logger, IUnitOfWorkRepository unitOfWorkRepository)
        {
            _logger = logger;
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public IActionResult Index(string? query = null)
        {
            IQueryable<Movie> movies = _unitOfWorkRepository.Movies.Get([e => e.Category, e => e.Cinema]).AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                movies = movies.Where(m => m.Name.Contains(query));

            }
            else
            {
                return View(movies);
            }

            return View(movies);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
    }
}
