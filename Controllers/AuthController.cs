using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using System.ComponentModel.DataAnnotations;


namespace MedicalTriageSystem.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => (u.Username == model.Username || u.Email == model.Username) && u.IsActive);

            if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                _logger.LogInformation("Connexion réussie : {Username} (Rôle: {Role})", user.Username, user.Role);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Dashboard", "Home");
            }

            ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterModel());  // Utilise Models.RegisterModel
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)  // Utilise Models.RegisterModel
        {
            if (!ModelState.IsValid)
                return View(model);

            // Vérifier si username ou email existe déjà
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Ce nom d'utilisateur ou email est déjà utilisé.");
                return View(model);
            }

            // Créer l'utilisateur
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Créer le profil patient si rôle Patient
            if (model.Role == "Patient")
            {
                var patient = new Patient
                {
                    UserId = user.Id,
                    Name = model.FullName ?? model.Username,
                    Email = model.Email,
                    Phone = model.Phone,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Compte créé avec succès ! Vous pouvez maintenant vous connecter.";
            return RedirectToAction("Login");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }

    public class LoginModel
    {
        [Required] public string Username { get; set; } = string.Empty;
        [Required][DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }

   
}