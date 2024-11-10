using E_TicketsCore.IUnitOfWorkRepository;
using E_TicketsCore.Models;
using E_TicketsCore.Utility;
using E_TicketsCore.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace E_Tickets.Controllers
{

    //[Authorize(Roles = $"{SD.adminRole}")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUnitOfWorkRepository _unitOfWorkRepository;


        public AccountController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                RoleManager<IdentityRole> roleManager, IUnitOfWorkRepository unitOfWorkRepository)

        
        {
           this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }


        [AllowAnonymous]

        public async Task <IActionResult> Register()
        {
            if (roleManager.Roles.IsNullOrEmpty())
            {
                await  roleManager.CreateAsync(new(SD.adminRole));
                await roleManager.CreateAsync(new(SD.companyRole));
                await roleManager.CreateAsync(new(SD.CustomerRole));
            }
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Register(ApplicationUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser()
                { UserName = userVM.Name, Email = userVM.Email, City = userVM.City };

                var result = await userManager.CreateAsync(applicationUser, userVM.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicationUser, SD.CustomerRole);
                    await signInManager.SignInAsync(applicationUser, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                }
            }
            return View(userVM);
        }
        [AllowAnonymous]

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var userDb=  await userManager.FindByNameAsync(loginVM.User);

                if (userDb != null)
                {
                    var finalResult = await userManager.CheckPasswordAsync(userDb, loginVM.Password);

                    if (finalResult)
                    {
                       //Login==> Create ID (Cookies)
                       await signInManager.SignInAsync(userDb, loginVM.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Invalid Password");
                    }
                }
                else
                {
                    ModelState.AddModelError("User", "Invalid User Name");
                }

            }
            return View();
        }
        [AllowAnonymous]

        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> Create(ApplicationUserVM userVM)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser()
                { UserName = userVM.Name, Email = userVM.Email, City = userVM.City };

                var result = await userManager.CreateAsync(applicationUser, userVM.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicationUser, SD.adminRole);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                }
            }
            return View(userVM);
        }

        public IActionResult Index()
        {
             var users= _unitOfWorkRepository.Users.Get(expression: e=>e.Email.Contains("@gmail")).ToList();
            return View(users);
        }

    }




}
