using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ReservasApp.WebAPI.Data;
using ReservasApp.WebAPI.Models;
using ReservasApp.WebAPI.Services;
using ReservasApp.WebAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ¿Por qué configurar tantos servicios? Para una API robusta y bien estructurada

#region Base de Datos y Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region Identity y Autenticación
builder.Services.AddIdentity<Usuario, IdentityRole>(options =>
{
    // Configuración de contraseñas
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    
    // Configuración de usuarios
    options.User.RequireUniqueEmail = true;
    
    // Configuración de bloqueo de cuenta
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// JWT Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey ?? throw new InvalidOperationException("JWT Secret key is not configured"))),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000", // React development server
            "http://localhost:5173", // Vite development server
            "http://localhost:5174", // Vite development server (puerto alternativo)
            "https://localhost:7096" // MVC application
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});
#endregion

#region AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
#endregion

#region Servicios y Repositorios
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IQRRepository, QRRepository>();
#endregion

#region Controllers y API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Reservas API",
        Version = "v1",
        Description = "API RESTful para sistema de reservas - Trabajo Práctico Programación 3",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Sistema de Reservas",
            Email = "admin@reservas.com"
        }
    });

    // Configuración para JWT en Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Ingrese 'Bearer' seguido de un espacio y el token JWT",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

var app = builder.Build();

#region Configuración del Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reservas API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

#region Inicialización de Datos
// Crear roles por defecto
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
    
    await SeedRolesAsync(roleManager);
    await SeedAdminUserAsync(userManager);
}
#endregion

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
