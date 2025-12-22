using MedicalTriageSystem.Data;
using MedicalTriageSystem.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Configure database context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? Microsoft.AspNetCore.Http.CookieSecurePolicy.None
            : Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
        options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
    });

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add SignalR
builder.Services.AddSignalR();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("DoctorOnly", policy => policy.RequireRole("Doctor"));
    options.AddPolicy("PatientOnly", policy => policy.RequireRole("Patient"));
});

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// IMPORTANT: UseAuthentication before UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");

// Initialiser la base de données
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Appliquer les migrations automatiquement
        context.Database.Migrate();

        Console.WriteLine("✅ Base de données initialisée avec succès!");

        // Vérifier les utilisateurs
        var userCount = await context.Users.CountAsync();
        Console.WriteLine($"👤 Nombre d'utilisateurs: {userCount}");

        // Créer un admin si nécessaire
        if (!await context.Users.AnyAsync(u => u.Username == "admin"))
        {
            var admin = new MedicalTriageSystem.Models.User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@triagemed.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            context.Users.Add(admin);
            await context.SaveChangesAsync();
            Console.WriteLine("🔑 Admin créé: admin / admin123");
        }

        // Créer un docteur si nécessaire
        if (!await context.Users.AnyAsync(u => u.Username == "docteur1"))
        {
            var doctorUser = new MedicalTriageSystem.Models.User
            {
                Id = 2,
                Username = "docteur1",
                Email = "docteur@clinique.fr",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("docteur123"),
                Role = "Doctor",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            context.Users.Add(doctorUser);

            var doctor = new MedicalTriageSystem.Models.Doctor
            {
                Id = 1,
                Name = "Dr. Martin Dubois",
                Specialty = "Médecine Générale",
                Phone = "01 23 45 67 89",
                Email = "m.dubois@clinique.fr",
                IsAvailable = true,
                Availability = "Lun-Ven: 9h-18h",
                UserId = 2
            };
            context.Doctors.Add(doctor);

            await context.SaveChangesAsync();
            Console.WriteLine("👨‍⚕️ Docteur créé: docteur1 / docteur123");
        }

        // Créer un patient si nécessaire
        if (!await context.Users.AnyAsync(u => u.Username == "patient1"))
        {
            var patientUser = new MedicalTriageSystem.Models.User
            {
                Id = 3,
                Username = "patient1",
                Email = "patient@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("patient123"),
                Role = "Patient",
                IsActive = true,
                CreatedAt = DateTime.Now
            };
            context.Users.Add(patientUser);

            var patient = new MedicalTriageSystem.Models.Patient
            {
                Id = 1,
                UserId = 3,
                Name = "Jean Dupont",
                Age = 35,
                Email = "patient@example.com",
                Phone = "06 12 34 56 78",
                Gender = "Homme",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            context.Patients.Add(patient);

            await context.SaveChangesAsync();
            Console.WriteLine("👤 Patient créé: patient1 / patient123");
        }

        // Vérifier les médecins
        var doctorCount = await context.Doctors.CountAsync();
        Console.WriteLine($"👨‍⚕️ Nombre de médecins: {doctorCount}");

        // Vérifier les patients
        var patientCount = await context.Patients.CountAsync();
        Console.WriteLine($"👤 Nombre de patients: {patientCount}");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erreur lors de l'initialisation: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"❌ Inner Exception: {ex.InnerException.Message}");
        }
    }
    // Dans Program.cs, juste avant app.Run()
    app.Map("/test-db", async context =>
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var html = new System.Text.StringBuilder();
        html.AppendLine("<html><head><style>");
        html.AppendLine("body { font-family: Arial; margin: 20px; }");
        html.AppendLine(".success { color: green; }");
        html.AppendLine(".error { color: red; }");
        html.AppendLine("pre { background: #f4f4f4; padding: 10px; }");
        html.AppendLine("</style></head><body>");
        html.AppendLine("<h1>Test de la base de données</h1>");

        try
        {
            // Test 1: Connexion
            html.AppendLine("<h2>1. Test de connexion</h2>");
            try
            {
                await dbContext.Database.OpenConnectionAsync();
                html.AppendLine("<p class='success'>✅ Connexion à la base réussie</p>");
                await dbContext.Database.CloseConnectionAsync();
            }
            catch (Exception ex)
            {
                html.AppendLine($"<p class='error'>❌ Erreur de connexion: {ex.Message}</p>");
            }

            // Test 2: Structure de la table Users
            html.AppendLine("<h2>2. Structure de la table Users</h2>");
            try
            {
                var columns = await dbContext.Database.SqlQueryRaw<string>(
                    "SELECT column_name || ' :: ' || data_type || ' ' || " +
                    "CASE WHEN is_nullable = 'NO' THEN 'NOT NULL' ELSE '' END " +
                    "FROM information_schema.columns " +
                    "WHERE table_name = 'Users' ORDER BY ordinal_position")
                    .ToListAsync();

                if (columns.Any())
                {
                    html.AppendLine("<ul>");
                    foreach (var column in columns)
                    {
                        html.AppendLine($"<li>{column}</li>");
                    }
                    html.AppendLine("</ul>");
                }
                else
                {
                    html.AppendLine("<p class='error'>❌ Table Users non trouvée</p>");
                }
            }
            catch (Exception ex)
            {
                html.AppendLine($"<p class='error'>❌ Erreur: {ex.Message}</p>");
            }

            // Test 3: Test d'insertion
            html.AppendLine("<h2>3. Test d'insertion</h2>");
            try
            {
                var testUsername = "test_" + Guid.NewGuid().ToString().Substring(0, 8);
                var testEmail = testUsername + "@test.com";

                // Insérez directement avec SQL pour éviter les problèmes EF
                var sql = $"INSERT INTO \"Users\" (\"Username\", \"Email\", \"PasswordHash\", \"Role\", \"IsActive\", \"CreatedAt\") " +
                         $"VALUES ('{testUsername}', '{testEmail}', '{BCrypt.Net.BCrypt.HashPassword("test123")}', 'Patient', true, CURRENT_TIMESTAMP)";

                var rowsAffected = await dbContext.Database.ExecuteSqlRawAsync(sql);

                if (rowsAffected > 0)
                {
                    html.AppendLine($"<p class='success'>✅ Insertion réussie: {testUsername}</p>");

                    // Nettoyer
                    await dbContext.Database.ExecuteSqlRawAsync($"DELETE FROM \"Users\" WHERE \"Username\" = '{testUsername}'");
                    html.AppendLine("<p class='success'>✅ Nettoyage réussi</p>");
                }
                else
                {
                    html.AppendLine("<p class='error'>❌ Aucune ligne insérée</p>");
                }
            }
            catch (Exception ex)
            {
                html.AppendLine($"<p class='error'>❌ Erreur d'insertion: {ex.Message}</p>");
                html.AppendLine($"<pre>{ex.StackTrace}</pre>");
            }

            // Test 4: Vérifier les contraintes
            html.AppendLine("<h2>4. Contraintes de la table Users</h2>");
            try
            {
                var constraints = await dbContext.Database.SqlQueryRaw<string>(
                    "SELECT conname || ' : ' || pg_get_constraintdef(oid) " +
                    "FROM pg_constraint " +
                    "WHERE conrelid = 'public.\"Users\"'::regclass")
                    .ToListAsync();

                if (constraints.Any())
                {
                    html.AppendLine("<ul>");
                    foreach (var constraint in constraints)
                    {
                        html.AppendLine($"<li>{constraint}</li>");
                    }
                    html.AppendLine("</ul>");
                }
                else
                {
                    html.AppendLine("<p>Aucune contrainte trouvée</p>");
                }
            }
            catch (Exception ex)
            {
                html.AppendLine($"<p class='error'>❌ Erreur: {ex.Message}</p>");
            }

            // Test 5: Données existantes
            html.AppendLine("<h2>5. Utilisateurs existants</h2>");
            try
            {
                var users = await dbContext.Users
                    .Select(u => new { u.Id, u.Username, u.Email, u.Role })
                    .Take(10)
                    .ToListAsync();

                if (users.Any())
                {
                    html.AppendLine("<table border='1' cellpadding='5'>");
                    html.AppendLine("<tr><th>ID</th><th>Username</th><th>Email</th><th>Role</th></tr>");
                    foreach (var user in users)
                    {
                        html.AppendLine($"<tr><td>{user.Id}</td><td>{user.Username}</td><td>{user.Email}</td><td>{user.Role}</td></tr>");
                    }
                    html.AppendLine("</table>");
                }
                else
                {
                    html.AppendLine("<p>Aucun utilisateur trouvé</p>");
                }
            }
            catch (Exception ex)
            {
                html.AppendLine($"<p class='error'>❌ Erreur: {ex.Message}</p>");
            }
        }
        catch (Exception ex)
        {
            html.AppendLine($"<h2 class='error'>❌ ERREUR GÉNÉRALE</h2>");
            html.AppendLine($"<p>{ex.Message}</p>");
            html.AppendLine($"<pre>{ex.StackTrace}</pre>");
        }

        html.AppendLine("</body></html>");
        await context.Response.WriteAsync(html.ToString());
    });
}

app.Run();