using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsCore.Utility;
using E_TicketsEF.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace E_Tickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole},{SD.CustomerRole}")]
    public class CartController : Controller
    {
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(IUnitOfWorkRepository unitOfWorkRepository, UserManager<ApplicationUser> userManager)
        {
            _unitOfWorkRepository = unitOfWorkRepository;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult AddToCart(int movieId, int count)
        {
            var appUser = _userManager.GetUserId(User);
            if (appUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (count < 1)
            {
                return RedirectToAction("Quantity");
            }
         
            var movie = _unitOfWorkRepository.Movies.GetOne(expression: m => m.Id == movieId);

            if (movie != null && movie.TicketsCount >= count)
            {
                Cart cart = new Cart()
                {
                    Count = count,
                    MovieId = movieId,
                    ApplicationUserId = appUser,
                };
                var cartDb = _unitOfWorkRepository.Carts.GetOne(expression: c => c.MovieId == movieId && c.ApplicationUserId == appUser);
                if (cartDb == null)
                {
                    _unitOfWorkRepository.Carts.Create(cart);
                    var trackSale = _unitOfWorkRepository.TrackingSales.CreateSpecial(cart);
                    trackSale.PaymentStatus = "Pending";
                    trackSale.Count = cart.Count;
                    _unitOfWorkRepository.TrackingSales.Create(trackSale);

                }
                else
                {
                    cartDb.Count += count;
                    var ExistingTrackSale = _unitOfWorkRepository.TrackingSales.GetOne(expression: c => c.MovieId == movieId && c.ApplicationUserId == appUser);
                    ExistingTrackSale.Count += count;

                }
                
                _unitOfWorkRepository.Complete();
                

                TempData["success"] = "the item has been added successfully";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Quantity");
            }

        }
        public IActionResult Index()
        {
            var appUser = _userManager.GetUserId(User);
            var ShoppingCarts = _unitOfWorkRepository.Carts
           .GetWithIncludes(x => x.ApplicationUserId == appUser, query => query
           .Include(m => m.Movie)
           .Include(m => m.ApplicationUser));
            ViewBag.TotalPrice = ShoppingCarts.Sum(e => e.Movie.Price * e.Count);
            return View(ShoppingCarts);
        }

        public IActionResult Increment(int movieId)

        {
            var appUser = _userManager.GetUserId(User);
            var cartDb = _unitOfWorkRepository.Carts.GetOne(expression: c => c.MovieId == movieId && c.ApplicationUserId == appUser);
            if (cartDb != null)
            {
                cartDb.Count++;
                _unitOfWorkRepository.Complete();
            }
            return RedirectToAction("Index");


        }
        public IActionResult Decrement(int movieId)

        {
            var appUser = _userManager.GetUserId(User);
            var cartDb = _unitOfWorkRepository.Carts.GetOne(expression: c => c.MovieId == movieId && c.ApplicationUserId == appUser);
            if (cartDb != null)
            {
                cartDb.Count--;
                if (cartDb.Count == 0)
                {
                    _unitOfWorkRepository.Carts.Delete(cartDb);
                }
                _unitOfWorkRepository.Complete();
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int movieId)
        {
            var appUser = _userManager.GetUserId(User);
            var cartDB = _unitOfWorkRepository.Carts.GetOne(expression: e => e.MovieId == movieId && e.ApplicationUserId == appUser);
            if (cartDB != null)
            {
                _unitOfWorkRepository.Carts.Delete(cartDB);
            }
            _unitOfWorkRepository.Complete();

            return RedirectToAction("Index");
        }
        [AllowAnonymous]

        public IActionResult Pay()
        {
            var appUser = _userManager.GetUserId(User);
            var cartDBs = _unitOfWorkRepository.Carts.Get(includeProps: [m => m.Movie], expression: e => e.ApplicationUserId == appUser).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Cart/PaymentSuccess",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/checkout/cancel",
            };

            foreach (var item in cartDBs)
            {
                var result = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "egp",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Movie.Name,
                        },
                        UnitAmount = (long)item.Movie.Price * 100,
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(result);
            }

            var service = new SessionService();
            var session = service.Create(options);

            return Redirect(session.Url);
        }
        [AllowAnonymous]

        public IActionResult PaymentSuccess()
        {
            var appUser = _userManager.GetUserId(User);
            var cartDBs = _unitOfWorkRepository.Carts.GetWithIncludes(e => e.ApplicationUserId == appUser, query => query.Include(e => e.Movie)).ToList();

            var trackingSalesList = _unitOfWorkRepository.TrackingSales.Get().ToList();
            foreach (var item in trackingSalesList)
            {
                item.PaymentStatus = "Done";
            }

            foreach (var cartDB in cartDBs)
            {
                cartDB.Movie.TicketsCount -= cartDB.Count;

                if (cartDB.Movie.TicketsCount < 0)

                { 
                    cartDB.Movie.TicketsCount = 0; 
                }

                _unitOfWorkRepository.Carts.Delete(cartDB);
            }
            _unitOfWorkRepository.Complete();


            TempData["success"] = "Payment was successful and tickets have been reserved!";
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Quantity()
        {
            return View();
        }
    }

}
