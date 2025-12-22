using MedicalTriageSystem.Data;
using MedicalTriageSystem.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MedicalTriageSystem.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Auth/Login
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToDashboard();
            }
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username || u.Email == username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    ModelState.AddModelError("", "Nom d'utilisateur ou mot de passe incorrect.");
                    return View();
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Ce compte est désactivé.");
                    return View();
                }

                // Create claims
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
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToDashboard();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Une erreur est survenue lors de la connexion.");
                Console.WriteLine($"Login error: {ex.Message}");
                return View();
            }
        }

        // GET: Auth/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if username exists
                    if (await _context.Users.AnyAsync(u => u.Username == model.Username))
                    {
                        ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà pris.");
                        return View(model);
                    }

                    // Check if email exists
                    if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                    {
                        ModelState.AddModelError("Email", "Cet email est déjà utilisé.");
                        return View(model);
                    }

                    // Create user
                    var user = new User
                    {
                        Username = model.Username,
                        Email = model.Email,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                        Role = model.Role,
                        IsActive = true,
                        CreatedAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // If patient role, create Patient record
                    if (model.Role == "Patient")
                    {
                        var patient = new Patient
                        {
                            UserId = user.Id,
                            Name = model.FullName ?? model.Username,
                            Email = model.Email,
                            Phone = model.Phone,
                            CreatedAt = DateTime.Now,
                            UpdatedAt = DateTime.Now
                        };

                        _context.Patients.Add(patient);
                        await _context.SaveChangesAsync();
                    }
                    // If doctor role, create Doctor record
                    else if (model.Role == "Doctor")
                    {
                        var doctor = new Doctor
                        {
                            UserId = user.Id,
                            Name = model.FullName ?? model.Username,
                            Email = model.Email,
                            Phone = model.Phone,
                            IsAvailable = true,
                            Specialty = "Médecine Générale"
                        };

                        _context.Doctors.Add(doctor);
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Inscription réussie ! Vous pouvez maintenant vous connecter.";
                    return RedirectToAction("Login");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Une erreur est survenue: {ex.Message}");
                Console.WriteLine($"Registration error: {ex.Message}");
                return View(model);
            }
        }

        // GET: Auth/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // GET: Auth/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToDashboard()
        {
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            return role switch
            {
                "Admin" => RedirectToAction("Index", "Dashboard"),
                "Doctor" => RedirectToAction("Index", "Dashboard"),
                "Patient" => RedirectToAction("Index", "PatientDashboard"),
                _ => RedirectToAction("Index", "Dashboard")
            };
        }
    }
}