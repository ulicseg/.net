using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasApp.MVC.Models;
using ReservasApp.MVC.Services;

namespace ReservasApp.MVC.Controllers;

[Authorize]
public class ReservasController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(
        IReservaService reservaService,
        UserManager<Usuario> userManager,
        ILogger<ReservasController> logger)
    {
        _reservaService = reservaService;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: /Reservas
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var reservas = await _reservaService.GetReservasPaginadasAsync(user.Id, 1, 100);
        return View(reservas);
    }

    // GET: /Reservas/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Reservas/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Reserva reserva)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Asegurar que la reserva quede asociada al usuario antes de validar
            reserva.UsuarioId = user.Id;

            // El campo UsuarioId no viene del formulario, quitemos su error (si lo hubiera)
            ModelState.Remove(nameof(Reserva.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(reserva);
            }

            await _reservaService.CrearReservaAsync(reserva);

            TempData["SuccessMessage"] = "¡Reserva creada exitosamente!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creando la reserva");
            TempData["ErrorMessage"] = "Ocurrió un error al crear la reserva.";
            return View(reserva);
        }
    }
} 