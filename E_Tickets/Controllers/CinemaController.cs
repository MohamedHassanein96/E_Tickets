using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole}")]
    public class CinemaController : Controller
    {   
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public CinemaController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult Index()
        {
            var cinemas = _unitOfWorkRepository.Cinemas.Get();
            return View(cinemas);
        }
        public IActionResult Details(int id)
        {
            var cinema = _unitOfWorkRepository.Cinemas.GetOne(expression: e => e.Id == id);
            return View(cinema);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cinema cinema, IFormFile cinemaLogo)
        {
            ModelState.Remove("CinemaLogo");
            if (ModelState.IsValid)
            {
                if (cinemaLogo != null && cinemaLogo.Length > 0)
                {
                    var fileName = Methods.UploadImg(cinemaLogo);
                    cinema.CinemaLogo = fileName;

                }
                _unitOfWorkRepository.Cinemas.Create(cinema);
                _unitOfWorkRepository.Complete();
                TempData["success"] = "the item has been added successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }
        public IActionResult Edit(int id)
        {
            var cinema = _unitOfWorkRepository.Cinemas.GetOne(null, e => e.Id == id);
            if (cinema != null)
            {
                return View(cinema);
            }
            else
            {
                return RedirectToAction("NotFound", "Movie");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cinema cinema, IFormFile newCinemaLogo)
        {
            var oldCinema = _unitOfWorkRepository.Cinemas.Get(null, e => e.Id == cinema.Id, false).FirstOrDefault();

            ModelState.Remove("newCinemaLogo");

            if (ModelState.IsValid)
            {
                if (newCinemaLogo != null && newCinemaLogo.Length > 0)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldCinema.CinemaLogo);
                    var fileName = Methods.UploadImg(newCinemaLogo);
                    cinema.CinemaLogo = fileName;
                }
                else
                {
                    cinema.CinemaLogo = oldCinema.CinemaLogo;
                }

                _unitOfWorkRepository.Cinemas.Edit(cinema);
                _unitOfWorkRepository.Complete();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                cinema.CinemaLogo = oldCinema.CinemaLogo;
                return View(cinema);
            }

        }
        public IActionResult Delete(int id)
        {
            var cinema = _unitOfWorkRepository.Cinemas.Get(expression: e => e.Id == id).FirstOrDefault();
            if (cinema != null)
            {
                _unitOfWorkRepository.Cinemas.Delete(cinema);
                _unitOfWorkRepository.Complete();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("NotFound", "Category");
            }

        }

        public IActionResult AllMovies(int id)
        {
            var cinemaMovies = _unitOfWorkRepository.Movies.GetWithIncludes(e => e.CinemaId == id,Query=>Query.Include(m=>m.Category));
            return View(cinemaMovies);
        }

    }
}
