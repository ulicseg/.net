using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasApp.MVC.Models;
using ReservasApp.MVC.Models.ViewModels;
using ReservasApp.MVC.Services;
using System.ComponentModel.DataAnnotations;

namespace ReservasApp.MVC.Controllers
{
    /// <summary>
    /// Controlador para manejo de autenticación
    /// ¿Por qué un controlador separado? Para mantener la lógica de auth organizada
    /// </summary>
    public class AccountController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager,
            IEmailService emailService,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
        }

        #region Registro

        [HttpGet]
        public IActionResult Register()
        {
            // Si ya está logueado, redirigir al dashboard
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // ¿Por qué? Protección contra CSRF attacks
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // Crear el usuario
                var usuario = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido,
                    FechaRegistro = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(usuario, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Email} registrado exitosamente", model.Email);

                    // Generar token de confirmación (opcional)
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(usuario);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", 
                        new { userId = usuario.Id, token }, Request.Scheme);

                    // Enviar email de confirmación (en desarrollo simula)
                    await _emailService.EnviarEmailConfirmacionAsync(usuario.Email!, token, callbackUrl!);

                    // Login automático después del registro
                    await _signInManager.SignInAsync(usuario, isPersistent: false);

                    TempData["SuccessMessage"] = "¡Registro exitoso! Bienvenido al sistema de reservas.";
                    return RedirectToAction("Index", "Dashboard");
                }

                // Si hay errores, mostrarlos
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en registro de usuario");
                ModelState.AddModelError(string.Empty, "Ocurrió un error durante el registro. Intenta nuevamente.");
            }

            return View(model);
        }

        #endregion

        #region Login

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Dashboard");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, 
                    model.Password, 
                    model.RememberMe, 
                    lockoutOnFailure: true); // ¿Por qué lockout? Protección contra ataques de fuerza bruta

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario {Email} inició sesión", model.Email);
                    
                    // Redirigir a donde estaba antes del login o al dashboard
                    var returnUrl = Request.Query["returnUrl"].ToString();
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
                    
                    return RedirectToAction("Index", "Dashboard");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, 
                        "Tu cuenta está bloqueada temporalmente por múltiples intentos fallidos.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en login");
                ModelState.AddModelError(string.Empty, "Error durante el inicio de sesión.");
            }

            return View(model);
        }

        #endregion

        #region Logout

        [HttpPost]
        [Authorize] // Solo usuarios logueados pueden hacer logout
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Usuario cerró sesión");
            TempData["InfoMessage"] = "Has cerrado sesión exitosamente.";
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Recuperación de Contraseña

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // No revelar si el usuario existe (seguridad)
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account",
                    new { userId = user.Id, token }, Request.Scheme);

                await _emailService.EnviarEmailRecuperacionAsync(user.Email!, token, callbackUrl!);

                return RedirectToAction("ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email de recuperación");
                ModelState.AddModelError(string.Empty, "Error enviando el email de recuperación.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            try
            {
                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reseteando contraseña");
                ModelState.AddModelError(string.Empty, "Error al restablecer la contraseña.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        #endregion

        #region Confirmación de Email (Opcional)

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string? userId, string? token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Home");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded);
        }

        #endregion

        #region Helpers

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #endregion
    }
}
