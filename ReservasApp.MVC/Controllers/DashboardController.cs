using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ReservasApp.MVC.Models;
using ReservasApp.MVC.Services;

namespace ReservasApp.MVC.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IReservaService _reservaService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            UserManager<Usuario> userManager,
            IReservaService reservaService,
            ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _reservaService = reservaService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Obtener reservas del usuario
                var reservas = await _reservaService.GetReservasPaginadasAsync(user.Id, 1, 100);
                
                // Estadísticas para el dashboard
                var proximasReservas = reservas.Where(r => r.FechaReserva > DateTime.Now).ToList();
                var reservasCompletadas = reservas.Where(r => r.FechaReserva < DateTime.Now).ToList();

                ViewBag.Usuario = user;
                ViewBag.TotalReservas = reservas.Count();
                ViewBag.ProximasReservas = proximasReservas.Count();
                ViewBag.ReservasCompletadas = reservasCompletadas.Count();
                ViewBag.ProximaReserva = proximasReservas.OrderBy(r => r.FechaReserva).FirstOrDefault();

                return View(reservas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cargando dashboard para usuario {UserId}", User.Identity?.Name);
                TempData["ErrorMessage"] = "Error cargando el panel principal.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
} 