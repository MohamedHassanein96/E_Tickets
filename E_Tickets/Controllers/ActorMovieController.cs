using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole}")]
    public class ActorMovieController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;

        public ActorMovieController(IUnitOfWorkRepository unitOfWorkRepository)
        {
           _unitOfWorkRepository = unitOfWorkRepository;
        }
        public IActionResult Index( )
        {
            var actorMovies = _unitOfWorkRepository.ActorMovies.GetWithIncludes
             (null,
             query => query
            .Include(ac => ac.Actor)
            .Include(ac => ac.Movie));
            return View(actorMovies);
        }

        public IActionResult Details(int actorId, int movieId)
        {
            ViewBag.movie= _unitOfWorkRepository.Movies.Get(null, m=>m.Id== movieId);
            ViewBag.actor= _unitOfWorkRepository.Actors.Get(null, a=>a.Id== actorId);

            var actorMovie = _unitOfWorkRepository.ActorMovies.GetByCompositeKeys<ActorMovie>(actorId, movieId);
            if (actorMovie != null)
            {
                return View(actorMovie);
            }
            else
            {
                return RedirectToAction("NotFound", "Category");

            }
        }
        public IActionResult Create()
        {
            ViewBag.Actors = _unitOfWorkRepository.Actors.Get().Select(ac => new SelectListItem
            {
                Value = ac.Id.ToString(),
                Text = ac.FirstName
            });
            ViewBag.Movies = _unitOfWorkRepository.Movies.Get().Select(m=> new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = m.Name
            });
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ActorMovie actorMovie)
        {

            if (ModelState.IsValid)
            {
                var existingEntry = _unitOfWorkRepository.ActorMovies.GetWithIncludes(am => am.ActorId == actorMovie.ActorId && am.MovieId == actorMovie.MovieId,null,false).FirstOrDefault();
                if (existingEntry != null)
                {
                    ViewBag.Actors = _unitOfWorkRepository.Actors.Get().Select(ac => new SelectListItem
                    {
                        Value = ac.Id.ToString(),
                        Text = ac.FirstName
                    });
                    ViewBag.Movies = _unitOfWorkRepository.Movies.Get().Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.Name
                    });
                    ModelState.AddModelError("MovieId", "This actor is already assigned to this movie.");
                    return View(actorMovie);

                }
                _unitOfWorkRepository.ActorMovies.Create(actorMovie);
                _unitOfWorkRepository.Complete();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Actors = _unitOfWorkRepository.Actors.Get().Select(ac => new SelectListItem
                {
                    Value = ac.Id.ToString(),
                    Text = ac.FirstName
                });
                ViewBag.Movies = _unitOfWorkRepository.Movies.Get().Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                });
                return View (actorMovie);
            }
            
        }

        public  IActionResult Edit(int actorId, int movieId)
        {
            var actorMovie = _unitOfWorkRepository.ActorMovies.GetByCompositeKeys<ActorMovie>(actorId, movieId);
            if (actorMovie != null)
            {
                ViewBag.Actors = _unitOfWorkRepository.Actors.Get().Select(ac => new SelectListItem
                {
                    Value = ac.Id.ToString(),
                    Text = ac.FirstName
                });
                ViewBag.Movies = _unitOfWorkRepository.Movies.Get().Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                });

                return View(actorMovie);
            }
            else
            {
                return RedirectToAction("NotFound", "Category");
            }
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit(ActorMovie actorMovie)
        {
            if (ModelState.IsValid)
            {
                _unitOfWorkRepository.ActorMovies.Edit(actorMovie);
                _unitOfWorkRepository.Complete();
                return RedirectToAction("Index");

            }
            else
            {
                ViewBag.Actors = _unitOfWorkRepository.Actors.Get().Select(ac => new SelectListItem
                {
                    Value = ac.Id.ToString(),
                    Text = ac.FirstName
                });
                ViewBag.Movies = _unitOfWorkRepository.Movies.Get().Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = m.Name
                });

                return View(actorMovie);
            }
        }

        public IActionResult Delete(int actorId, int movieId)
        {
            var actorMovie = _unitOfWorkRepository.ActorMovies.GetByCompositeKeys<ActorMovie>(actorId, movieId);
            if (actorMovie != null)
            {
                _unitOfWorkRepository.ActorMovies.Delete(actorMovie);
                _unitOfWorkRepository.Complete();
                return RedirectToAction("Index");
            }
            return RedirectToAction("NotFound", "Category");
        }


    }
}
