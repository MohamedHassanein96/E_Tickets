using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using static E_TicketsCore.Models.Movie;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole} ,{SD.companyRole}")]
    public class MovieController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        private void PopulateCategoriesAndCinemas()
        {
            var categories = _unitOfWorkRepository.Categories.Get()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
            ViewBag.categories = categories;

            var cinemas = _unitOfWorkRepository.Cinemas.Get()
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
            ViewBag.cinemas = cinemas;
        }
        public void SetMovieStatus(Movie movie)
        {
            DateTime currentDate = DateTime.Now;
            if (currentDate < movie.StartDate)
            {
                movie.Status = Movie.MovieStatus.Upcoming;
            }

            else if (currentDate >= movie.StartDate && currentDate <= movie.EndDate)
            {
                movie.Status = Movie.MovieStatus.Available;
            }
            else
            {
                movie.Status = Movie.MovieStatus.NotAvailable;
            }

        }

        public MovieController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult Index()
        {

            var movies = _unitOfWorkRepository.Movies.Get([e => e.Category, e => e.Cinema]);
            return View(movies.ToList());
        }
        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var movie = _unitOfWorkRepository.Movies.GetWithIncludes
                (m => m.Id == id, query => query
                .Include(m => m.Category)
                .Include(e=>e.Cinema)
                .Include(m => m.ActorMovies)
                .ThenInclude(am => am.Actor))
                .FirstOrDefault();
            return View(movie);
        }

        public IActionResult Edit(int id)
        {
            PopulateCategoriesAndCinemas();

            var movie = _unitOfWorkRepository.Movies
                .GetWithIncludes(m => m.Id == id,
                 Query => Query.Include(m => m.Category)
                .Include(m=>m.Cinema)).FirstOrDefault();

            if (movie != null)
            {
                return View(movie);
            }
            else
            {
                return RedirectToAction("NotFound", "Category");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie, IFormFile newImageUrl)
        {
            var oldMovie = _unitOfWorkRepository.Movies.Get(null, e => e.Id == movie.Id, false).FirstOrDefault();

            ModelState.Remove("newImageUrl");
            ModelState.Remove("StartDate");
            ModelState.Remove("EndDate");
           
            if (ModelState.IsValid)
            {
                if (movie.EndDate <= movie.StartDate)
                {
                    PopulateCategoriesAndCinemas();
                    ModelState.AddModelError("EndDate", "EndDate Mustn't Precede The StartDate");
                    return View(movie);
                }
                SetMovieStatus(movie);
                if (newImageUrl != null && newImageUrl.Length > 0)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldMovie.ImgUrl);
                    var fileName = Methods.UploadImg(newImageUrl);
                    movie.ImgUrl = fileName;
                }
                else
                {
                    movie.ImgUrl = oldMovie.ImgUrl;
                }

                _unitOfWorkRepository.Movies.Edit(movie);
                _unitOfWorkRepository.Complete();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                PopulateCategoriesAndCinemas();
                var movies = _unitOfWorkRepository.Movies.Get().Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });
                ViewBag.movies = movies;
                movie.ImgUrl = oldMovie.ImgUrl;
                return View(movie);
            }
        }

        public IActionResult Create()
        {
            PopulateCategoriesAndCinemas();
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Movie movie, IFormFile imageUrl)
        {

            ModelState.Remove("ImgUrl");
            ModelState.Remove("imageUrl");

            if (ModelState.IsValid)
            {
                if (movie.EndDate<=movie.StartDate)
                {
                    PopulateCategoriesAndCinemas();
                    ModelState.AddModelError("EndDate", "EndDate Mustn't Precede The StartDate");
                    return View(movie);
                }

                SetMovieStatus(movie);
                if (imageUrl != null && imageUrl.Length > 0)
                {
                    var fileName = Methods.UploadImg(imageUrl);
                    movie.ImgUrl = fileName;
                }
                else
                {
                    PopulateCategoriesAndCinemas();
                    ModelState.AddModelError("ImgUrl", " ImageUrl is Required");
                    return View(movie);

                }
                _unitOfWorkRepository.Movies.Create(movie);
                _unitOfWorkRepository.Complete();
                TempData["success"] = "the item has been added successfully";
                return RedirectToAction(nameof(Index));
            }

            else
            {  
                PopulateCategoriesAndCinemas();
                return View(movie);
            }

        }
        public IActionResult Delete(int id)
        {
            var movie = _unitOfWorkRepository.Movies.Get(expression: e => e.Id == id).FirstOrDefault();
            if (movie != null)
            {
                _unitOfWorkRepository.Movies.Delete(movie);
                _unitOfWorkRepository.Complete();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction("NotFound", "Category");
            }

        }
    }
}
