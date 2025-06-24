using Messenger.App.DTOs;
using Messenger.App.Services;
using Microsoft.AspNetCore.Mvc;


namespace Messenger.Api.Controllers
{
    public class UserController : Controller
    {
        public readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _userService.Login(loginDTO);
            if (user != null)
            {
                return RedirectToAction("GetUsers", "User", new { currentUserId = user.Id });
            }

            return View();
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegistrationDTO());
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegistrationDTO registrationDTO)
        {
            var validationResult = await _userService.Register(registrationDTO);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                return View(registrationDTO);
            }

            return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(Guid currentUserId)
        {
            var users = await _userService.GetUsers(currentUserId);

            if (users == null)
            {
                users = new List<GetUsersDTO>();
            }

            ViewBag.CurrentUserId = currentUserId;
            return View(users);
        }
    }
}
