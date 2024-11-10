using E_TicketsCore.IRepository;
using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole}")]

    public class CategoryController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public CategoryController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public IActionResult Index()
        {
           var categories= _unitOfWorkRepository.Categories.Get();
			return View(categories);
        }
        public IActionResult Details(int id)
        {
            var cinemaMovies = _unitOfWorkRepository.Movies.GetWithIncludes(e => e.CinemaId == id);
            return View(cinemaMovies);
        }
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkRepository.Categories.Create(category);
                _unitOfWorkRepository.Complete();
                TempData["success"] = "the item has been added successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
        public IActionResult Edit(int categoryId)
        {
            var category = _unitOfWorkRepository.Categories.Get(expression: e => e.Id == categoryId).FirstOrDefault();
            if (category!=null)
            {
                return View(category);
            }
            else
            {
                return RedirectToAction("NotFound", "Category");
            }

        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkRepository.Categories.Edit(category);
                _unitOfWorkRepository.Complete();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(category);
            }
        }

        public  new IActionResult  NotFound()
        {
            return View();
        }
        [AutoValidateAntiforgeryToken]

        public IActionResult Delete(int id)
        {
            var category = _unitOfWorkRepository.Categories.Get(expression: e => e.Id == id).FirstOrDefault();
            if (category != null)
            {
                _unitOfWorkRepository.Categories.Delete(category);
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
            var categoryMovies = _unitOfWorkRepository.Movies.GetWithIncludes(e => e.CategoryId == id, Query => Query.Include(m => m.Category));
            return View(categoryMovies);
        }
    }

}
