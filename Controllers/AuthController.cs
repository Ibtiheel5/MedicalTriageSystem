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

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            // Log pour voir si la page est appelée
            Console.WriteLine("=== PAGE REGISTER CHARGÉE ===");

            var model = new RegisterModel
            {
                Role = "Patient" // Par défaut
            };

            return View(model);
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            try
            {
                Console.WriteLine("=== TENTATIVE D'INSCRIPTION ===");
                Console.WriteLine($"Données reçues:");
                Console.WriteLine($"- Username: {model?.Username}");
                Console.WriteLine($"- Email: {model?.Email}");
                Console.WriteLine($"- Password length: {model?.Password?.Length}");
                Console.WriteLine($"- ConfirmPassword length: {model?.ConfirmPassword?.Length}");
                Console.WriteLine($"- Role: {model?.Role}");
                Console.WriteLine($"- FullName: {model?.FullName}");
                Console.WriteLine($"- Phone: {model?.Phone}");

                // Log ModelState avant validation
                Console.WriteLine($"ModelState.IsValid avant: {ModelState.IsValid}");
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    if (state.Errors.Any())
                    {
                        Console.WriteLine($"  {key}: {string.Join(", ", state.Errors.Select(e => e.ErrorMessage))}");
                    }
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("❌ MODELSTATE INVALIDE - Afficher les erreurs");

                    // Log détaillé des erreurs
                    foreach (var error in ModelState)
                    {
                        if (error.Value.Errors.Any())
                        {
                            Console.WriteLine($"  Champ '{error.Key}':");
                            foreach (var err in error.Value.Errors)
                            {
                                Console.WriteLine($"    - {err.ErrorMessage}");
                            }
                        }
                    }

                    return View(model);
                }

                Console.WriteLine("✅ ModelState valide");

                // Normalisation
                var username = model.Username?.Trim();
                var email = model.Email?.Trim().ToLower();

                // VÉRIFICATION MANUELLE ET EXPLICITE
                Console.WriteLine("🔍 Vérification des doublons...");

                var usernameExists = await _context.Users.AnyAsync(u => u.Username == username);
                if (usernameExists)
                {
                    Console.WriteLine($"❌ Username '{username}' existe déjà");
                    ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà utilisé.");
                }

                var emailExists = await _context.Users.AnyAsync(u => u.Email == email);
                if (emailExists)
                {
                    Console.WriteLine($"❌ Email '{email}' existe déjà");
                    ModelState.AddModelError("Email", "Cet email est déjà utilisé.");
                }

                // Vérification des mots de passe
                if (model.Password != model.ConfirmPassword)
                {
                    Console.WriteLine("❌ Les mots de passe ne correspondent pas");
                    ModelState.AddModelError("ConfirmPassword", "Les mots de passe ne correspondent pas.");
                }

                // Si erreurs, retourner à la vue
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("❌ Erreurs de validation détectées");
                    return View(model);
                }

                // CRÉATION DE L'UTILISATEUR
                Console.WriteLine("🔨 Création de l'utilisateur...");

                var user = new User
                {
                    Username = username,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    Role = string.IsNullOrEmpty(model.Role) ? "Patient" : model.Role,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                Console.WriteLine($"  Username: {user.Username}");
                Console.WriteLine($"  Email: {user.Email}");
                Console.WriteLine($"  Role: {user.Role}");
                Console.WriteLine($"  PasswordHash généré: {!string.IsNullOrEmpty(user.PasswordHash)}");

                // SAUVEGARDE
                _context.Users.Add(user);
                Console.WriteLine("✅ Utilisateur ajouté au DbContext");

                try
                {
                    var saveResult = await _context.SaveChangesAsync();
                    Console.WriteLine($"✅ SaveChangesAsync: {saveResult} enregistrement(s)");
                    Console.WriteLine($"✅ ID généré: {user.Id}");
                }
                catch (DbUpdateException dbEx)
                {
                    Console.WriteLine($"❌ ERREUR BDD: {dbEx.Message}");
                    Console.WriteLine($"   Inner: {dbEx.InnerException?.Message}");

                    // Gestion des erreurs spécifiques
                    if (dbEx.InnerException?.Message?.Contains("23505") == true) // Violation UNIQUE
                    {
                        if (dbEx.InnerException.Message.Contains("Username"))
                        {
                            ModelState.AddModelError("Username", "Ce nom d'utilisateur est déjà utilisé.");
                        }
                        else if (dbEx.InnerException.Message.Contains("Email"))
                        {
                            ModelState.AddModelError("Email", "Cet email est déjà utilisé.");
                        }
                    }
                    else if (dbEx.InnerException?.Message?.Contains("23503") == true) // Violation FK
                    {
                        ModelState.AddModelError(string.Empty, "Erreur de référence. Veuillez contacter l'administrateur.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Erreur de base de données. Code: " + dbEx.InnerException?.Message);
                    }

                    return View(model);
                }
                catch (Exception saveEx)
                {
                    Console.WriteLine($"❌ ERREUR SaveChanges: {saveEx.Message}");
                    Console.WriteLine($"   Stack: {saveEx.StackTrace}");
                    ModelState.AddModelError(string.Empty, "Erreur lors de la sauvegarde: " + saveEx.Message);
                    return View(model);
                }

                // CRÉATION DU PROFIL PATIENT SI BESOIN
                if (user.Role == "Patient")
                {
                    Console.WriteLine("🔨 Création du profil patient...");

                    var patient = new Patient
                    {
                        UserId = user.Id,
                        Name = !string.IsNullOrEmpty(model.FullName) ? model.FullName.Trim() : user.Username,
                        Age = 0, // À compléter plus tard
                        Email = user.Email,
                        Phone = !string.IsNullOrEmpty(model.Phone) ? model.Phone.Trim() : null,
                        Gender = "Non spécifié", // À compléter plus tard
                        BloodType = "Non spécifié",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Patients.Add(patient);

                    try
                    {
                        var patientSaveResult = await _context.SaveChangesAsync();
                        Console.WriteLine($"✅ Patient créé: {patientSaveResult} enregistrement(s)");
                        Console.WriteLine($"✅ Patient ID: {patient.Id}");
                    }
                    catch (Exception patientEx)
                    {
                        // Si erreur sur patient, on continue quand même (l'utilisateur est créé)
                        Console.WriteLine($"⚠️ Erreur création patient: {patientEx.Message}");
                        // On ne bloque pas l'inscription pour ça
                    }
                }

                // SUCCÈS - CONNEXION AUTOMATIQUE
                Console.WriteLine("🎉 INSCRIPTION RÉUSSIE - Connexion automatique...");

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString()),
                    new Claim("FullName", model.FullName ?? user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24) // 24h de session
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                Console.WriteLine($"✅ Utilisateur connecté: {user.Username} ({user.Role})");

                // REDIRECTION INTELLIGENTE
                string redirectUrl = user.Role switch
                {
                    "Patient" => "/Patient/Dashboard",
                    "Doctor" => "/Doctor/Dashboard",
                    "Admin" => "/Admin/Dashboard",
                    _ => "/Home"
                };

                Console.WriteLine($"🔀 Redirection vers: {redirectUrl}");

                // Message de bienvenue
                TempData["WelcomeMessage"] = $"Bienvenue {model.FullName ?? user.Username} ! Votre compte a été créé avec succès.";

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 ERREUR GÉNÉRALE INSCRIPTION: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                    Console.WriteLine($"Inner Stack: {ex.InnerException.StackTrace}");
                }

                ModelState.AddModelError(string.Empty,
                    "Une erreur inattendue s'est produite. " +
                    "Veuillez réessayer ou contacter le support si le problème persiste.");

                return View(model);
            }
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null, string message = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Message"] = message;
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Username);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Nom d'utilisateur ou email incorrect.");
                    return View(model);
                }

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    ModelState.AddModelError(string.Empty, "Mot de passe incorrect.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Ce compte est désactivé.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim("UserId", user.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(24)
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                Console.WriteLine($"✅ Connexion réussie: {user.Username}");

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                // Redirection par rôle
                return user.Role switch
                {
                    "Patient" => RedirectToAction("Dashboard", "Patient"),
                    "Doctor" => RedirectToAction("Dashboard", "Doctor"),
                    "Admin" => RedirectToAction("Index", "Dashboard"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur connexion: {ex.Message}");
                ModelState.AddModelError(string.Empty, "Erreur de connexion. Veuillez réessayer.");
                return View(model);
            }
        }

        // POST: /Auth/Logout
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine("👋 Utilisateur déconnecté");
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult IsAuthenticated()
        {
            return Json(new { isAuthenticated = User.Identity?.IsAuthenticated ?? false });
        }
    }

    // Modèle pour le login
    public class LoginModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur ou email est requis")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}