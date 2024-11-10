using E_TicketsCore.IRepository;
using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsCore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole}")]
    public class ActorController : Controller
    {

        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        public ActorController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }
        //[AllowAnonymous]
      
        public IActionResult Index(int id)
        {
            var actors = _unitOfWorkRepository.Actors.Get();
            return View(actors);
        }

        public IActionResult Details(int id)
        {
            var ActorMovie = _unitOfWorkRepository.Actors.GetWithIncludes
             (e => e.Id == id, query => query.Include(ac => ac.ActorMovies)
             .ThenInclude(m => m.Movie)).FirstOrDefault();
            return View(ActorMovie);
        }

        public IActionResult Edit(int id)
        {

            var actor = _unitOfWorkRepository.Actors.GetOne(null, e => e.Id == id);
            if (actor != null)
            {
                return View(actor);
            }
            else
            {
                return RedirectToAction("NotFound", "Movie");
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Actor actor, IFormFile newProfilePicture)
        {
            var oldActor = _unitOfWorkRepository.Actors.Get(null, e => e.Id == actor.Id, false).FirstOrDefault();

            ModelState.Remove("newProfilePicture");

            if (ModelState.IsValid)
            {
                if (newProfilePicture != null && newProfilePicture.Length > 0)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", oldActor.ProfilePicture);
                    var fileName = Methods.UploadImg(newProfilePicture);
                    actor.ProfilePicture = fileName;
                }
                else
                {
                    actor.ProfilePicture = oldActor.ProfilePicture;
                }

                _unitOfWorkRepository.Actors.Edit(actor);
                _unitOfWorkRepository.Complete();
                TempData["success"] = "the item has been added successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                actor.ProfilePicture = oldActor.ProfilePicture;
                return View(actor);
            }

        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Create(Actor actor, IFormFile profilePicture)
        {
            
            ModelState.Remove("ProfilePicture");

            if (ModelState.IsValid)
            {
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    var fileName =Methods.UploadImg(profilePicture);
                    actor.ProfilePicture = fileName;
                    _unitOfWorkRepository.Actors.Create(actor);
                    _unitOfWorkRepository.Complete();
                    TempData["success"] = "the item has been added successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("ProfilePicture", " ProfilePicture is Required");
                    return View(actor);
                }

            }
            else
            {
                return View(actor);
            }
            

        }

        public IActionResult Delete(int id)
        {
            var Actor = _unitOfWorkRepository.Actors.Get(expression: e => e.Id == id).FirstOrDefault();
            if (Actor != null)
            {
                _unitOfWorkRepository.Actors.Delete(Actor);
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
