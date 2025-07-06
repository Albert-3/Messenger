using Messenger.App.DTOs;
using Messenger.App.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Messenger.Api.Controllers
{
    public class UserController : Controller
    {
        public readonly UserService _userService;
        public readonly EmailService _emailService;
        public UserController(UserService userService ,EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
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
            if (user == null)
            {
                ModelState.AddModelError("", "Incorrect Login or Password");
                return View(loginDTO);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier.ToString(), user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
             };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30) });

            return RedirectToAction("GetUsers", "User");
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
            string subject = "Welcome ChatHUB!";
            string body = $"Hello, {registrationDTO.UserName}! Thank you for registering.";

            await _emailService.SendEmailAsync(registrationDTO.Email, subject, body);

            return RedirectToAction("Login", "User");
        }

        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var currentUser = await _userService.GetUserById(Guid.Parse(userId));
            var users = await _userService.GetUsers(Guid.Parse(userId));
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(); 
            }
            if (users == null)
            {
                users = new List<GetUsersDTO>();
            }

            ViewBag.CurrentUserId = currentUser.Id;

            return View(users);
        }
    }
}
