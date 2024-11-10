using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Tickets.Controllers
{
    public class TrackingSaleController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;



        public TrackingSaleController(IUnitOfWorkRepository unitOfWorkRepository)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
        }

        public IActionResult Index()
        {
            var trackingSales = _unitOfWorkRepository.TrackingSales.Get();
            return View(trackingSales);
        }

        public IActionResult Delete(int id)
        {
            var trackingSale = _unitOfWorkRepository.TrackingSales.Get(null,e=>e.Id==id).FirstOrDefault();
            if (trackingSale!=null)
            {
                _unitOfWorkRepository.TrackingSales.Delete(trackingSale);
                _unitOfWorkRepository.Complete();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                return RedirectToAction("NotFound", "Category");

            }

        }
        public IActionResult Details(int id)
        {
            var trackingSale = _unitOfWorkRepository.TrackingSales.Get(null, e => e.Id == id).FirstOrDefault();
            if (trackingSale != null)
            {
                return View(trackingSale);
            }
            else
            {
                return RedirectToAction("NotFound", "Category");

            }
        }

    }
}
