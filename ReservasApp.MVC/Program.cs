using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservasApp.MVC.Data;
using ReservasApp.MVC.Models;
using ReservasApp.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// ¿Por qué configuramos Entity Framework primero?
// Porque otros servicios (como Identity) dependen del contexto de base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? 
        "Data Source=reservas.db"));

// Configuración de ASP.NET Identity
// ¿Por qué estas configuraciones específicas?
// - Password: Para cumplir con buenas prácticas de seguridad
// - Lockout: Para proteger contra ataques de fuerza bruta
// - User: Para permitir emails como nombres de usuario
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
    
    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false; // Cambiar a true en producción
    
    // Configuración de bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders(); // Para recuperación de contraseñas

// Servicios personalizados
// ¿Por qué servicios separados? Para seguir el principio de responsabilidad única
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IQRService, QRService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configuración de autenticación con cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configuración de logging para depuración
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// Crear base de datos y aplicar migraciones al inicio
// ¿Por qué aquí? Para asegurar que la BD existe antes de usarla
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated(); // Crear BD si no existe
    
    // Inicializar roles del sistema
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
    
    await SeedRolesAsync(roleManager);
    await SeedAdminUserAsync(userManager);
    
    // En producción usarías: context.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection(); // Solo usar HTTPS en producción
}
else
{
    app.UseDeveloperExceptionPage(); // Páginas de error detalladas en desarrollo
}
app.UseStaticFiles(); // Para servir archivos CSS, JS, imágenes

app.UseRouting();

// ¡IMPORTANTE! El orden importa en ASP.NET Core
// 1. Autenticación (¿quién eres?)
app.UseAuthentication();
// 2. Autorización (¿qué puedes hacer?)
app.UseAuthorization();

// Rutas personalizadas para QR
app.MapControllerRoute(
    name: "qr",
    pattern: "QR/{hash}",
    defaults: new { controller = "QR", action = "Access" });

// Rutas para el área de administración (si la implementas)
app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Index}",
    defaults: new { controller = "Admin" });

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

#region Métodos de Inicialización
/// <summary>
/// Crea los roles básicos del sistema
/// ¿Por qué roles fijos? Para simplificar la gestión de permisos
/// </summary>
static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roles = { "Admin", "Cliente" };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

/// <summary>
/// Crea un usuario administrador por defecto
/// ¿Por qué crear admin por defecto? Para poder gestionar el sistema desde el inicio
/// </summary>
static async Task SeedAdminUserAsync(UserManager<Usuario> userManager)
{
    const string adminEmail = "admin@reservas.com";
    const string adminPassword = "Admin123!";
    
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new Usuario
        {
            UserName = adminEmail,
            Email = adminEmail,
            Nombre = "Administrador",
            Apellido = "Sistema",
            EmailConfirmed = true,
            FechaRegistro = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(adminUser, adminPassword);
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
#endregion
