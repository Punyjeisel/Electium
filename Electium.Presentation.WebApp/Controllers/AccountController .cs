using Electium.Core.Application.DTOs;
using Electium.Core.Domain.Enums;
using Electium.Presentation.WebApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using IAuthenticationService = Electium.Core.Application.Interfaces.IAuthenticationService;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Electium.Presentation.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationService _authService;

        public AccountController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _authService.LoginAsync(new LoginRequestDto
            {
                UserName = model.UserName,
                Password = model.Password
            });

            if (response.Result != LoginResult.Success)
            {
                ModelState.AddModelError("", GetErrorMessage(response.Result));
                return View(model);
            }

            await SignInAsync(response);
            return RedirectToHomeFor(response.Role!.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // --- private helpers (web concern only, not business logic) ---

        private async Task SignInAsync(LoginResponseDto user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId!.Value.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(ClaimTypes.Role, user.Role!.Value.ToString())
        };

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }

        private IActionResult RedirectToHomeFor(UserRole role)
        {
            return role == UserRole.Administrator
                ? RedirectToAction("Index", "AdminHome")
                : RedirectToAction("Index", "LeaderHome");
        }

        private static string GetErrorMessage(LoginResult result) => result switch
        {
            LoginResult.InvalidCredentials => "Los datos de acceso son inválidos.",
            LoginResult.UserInactive => "El usuario está inactivo.",
            LoginResult.LeaderWithoutParty =>
                "No tiene un partido político asignado, por lo tanto no puede iniciar sesión. " +
                "Por favor, póngase en contacto con un administrador.",
            _ => "No fue posible iniciar sesión."
        };
    }
}
