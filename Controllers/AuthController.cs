using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using MedicalTriageSystem.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
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
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Identifiants incorrects");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Compte désactivé");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation($"User {user.Username} logged in");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Dashboard", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error");
                ModelState.AddModelError("", "Une erreur est survenue");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // 1. Vérifier si l'utilisateur existe déjà
                if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                {
                    ModelState.AddModelError("Username", "Ce nom d'utilisateur existe déjà");
                    return View(model);
                }

                // 2. Créer l'objet User (avec UpdatedAt hérité de BaseEntity)
                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = model.Role ?? "Patient",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                // Première sauvegarde pour générer l'ID de l'utilisateur
                await _context.SaveChangesAsync();

                // 3. Création automatique du profil selon le rôle choisi
                if (user.Role == "Doctor")
                {
                    var doctor = new Doctor
                    {
                        UserId = user.Id, // Lien crucial avec l'utilisateur ID 1, 2, etc.
                        Name = model.FullName ?? user.Username,
                        Email = user.Email,
                        Specialty = "Généraliste", // Valeur par défaut requise
                        LicenseNumber = "REG-" + Guid.NewGuid().ToString().Substring(0, 8),
                        Phone = model.Phone ?? "71000000",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Doctors.Add(doctor);
                }
                else
                {
                    var patient = new Patient
                    {
                        UserId = user.Id,
                        Name = model.FullName ?? user.Username,
                        Email = user.Email,
                        Phone = model.Phone ?? "",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _context.Patients.Add(patient);
                }

                // 4. Sauvegarde finale des profils
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Compte créé avec succès !";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'inscription");
                ModelState.AddModelError("", "Une erreur est survenue lors de la création du profil : " + (ex.InnerException?.Message ?? ex.Message));
                return View(model);
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user != null)
            {
                // Generate reset token and send email
                var resetToken = Guid.NewGuid().ToString();
                // Store resetToken in database with expiry
                // Send email with reset link
            }

            // Always show success message for security
            TempData["InfoMessage"] = "Si votre email existe dans notre système, vous recevrez un lien de réinitialisation.";
            return RedirectToAction("Login");
        }
    }
}