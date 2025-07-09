using Messenger.App.DTOs;
using Messenger.App.Services;
using Messenger.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace Messenger.Api.Controllers
{
    public class UserController : Controller
    {
        public readonly UserService _userService;
        public readonly EmailService _emailService;
        public readonly IMemoryCache _cache;
        public UserController(UserService userService,
            EmailService emailService,
            IMemoryCache cache)
        {
            _userService = userService;
            _emailService = emailService;
            _cache = cache;
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
            if (_cache.TryGetValue($"Token_{user.UserName}", out _))
            {
                ModelState.AddModelError("", "Your email is not confirmed yet. Please check your inbox.");
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
        public async Task<IActionResult> Register(RegistrationDTO registrationDTO, CancellationToken cancellation = default)
        {
            var validationResult = await _userService.Register(registrationDTO);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);

                return View(registrationDTO);
            }

            var token = Guid.NewGuid().ToString();

            _cache.Set(token, registrationDTO, TimeSpan.FromHours(15));
            _cache.Set($"Token_{registrationDTO.UserName}", registrationDTO, TimeSpan.FromHours(15));

            await _userService.SendConfirmationEmailAsync(registrationDTO, token, cancellation);

            return RedirectToAction("ConfirmEmailNotice", new { email = registrationDTO.Email, userName = registrationDTO.UserName });
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
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing or invalid.");
            }

            var result = await _userService.ConfirmEmailAsync(token);
            if (!result)
            {
                return NotFound("Invalid or expired token.");
            }

            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult ConfirmEmailNotice(string email, string userName)
        {
            ViewBag.Email = email;
            ViewBag.UserName = userName;
            return View();
        }

    }
}
