using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservasApp.MVC.Data;
using ReservasApp.MVC.Models;
using ReservasApp.MVC.Services;

namespace ReservasApp.MVC.Controllers;

[Authorize]
public class ReservasController : Controller
{
    private readonly IReservaService _reservaService;
    private readonly IQRService _qrService;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Usuario> _userManager;
    private readonly ILogger<ReservasController> _logger;

    public ReservasController(
        IReservaService reservaService,
        IQRService qrService,
        ApplicationDbContext context,
        UserManager<Usuario> userManager,
        ILogger<ReservasController> logger)
    {
        _reservaService = reservaService;
        _qrService = qrService;
        _context = context;
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

    // GET: /Reservas/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var reserva = await _reservaService.GetReservaPorIdAsync(id, user.Id);
        if (reserva == null)
        {
            return NotFound();
        }

        return View(reserva);
    }

    // GET: /Reservas/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var reserva = await _reservaService.GetReservaPorIdAsync(id, user.Id);
        if (reserva == null)
        {
            return NotFound();
        }

        return View(reserva);
    }

    // POST: /Reservas/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Reserva reserva)
    {
        if (id != reserva.Id)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            reserva.UsuarioId = user.Id;
            ModelState.Remove(nameof(Reserva.UsuarioId));

            if (!ModelState.IsValid)
            {
                return View(reserva);
            }

            var success = await _reservaService.ActualizarReservaAsync(reserva, user.Id);
            if (success)
            {
                TempData["SuccessMessage"] = "¡Reserva actualizada exitosamente!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo actualizar la reserva.";
                return View(reserva);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error actualizando la reserva");
            TempData["ErrorMessage"] = "Ocurrió un error al actualizar la reserva.";
            return View(reserva);
        }
    }

    // POST: /Reservas/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            var success = await _reservaService.EliminarReservaAsync(id, user.Id);
            if (success)
            {
                TempData["SuccessMessage"] = "¡Reserva eliminada exitosamente!";
            }
            else
            {
                TempData["ErrorMessage"] = "No se pudo eliminar la reserva.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error eliminando la reserva");
            TempData["ErrorMessage"] = "Ocurrió un error al eliminar la reserva.";
        }

        return RedirectToAction("Index");
    }

    // POST: /Reservas/GenerateQR/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateQR(int id, string? returnUrl = null)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        try
        {
            // Verificar que la reserva pertenece al usuario
            var reserva = await _reservaService.GetReservaPorIdAsync(id, user.Id);
            if (reserva == null)
            {
                TempData["ErrorMessage"] = "Reserva no encontrada.";
                return RedirectToAction("Index");
            }

            // Generar QR
            var qrLink = await _qrService.CrearQRParaReservaAsync(id, "VerDetalle");
            
            // Generar la URL completa del QR (apuntando al MVC por defecto)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var qrUrl = $"{baseUrl}/Reservas/QRAccess/{qrLink.Hash}";
            
            TempData["SuccessMessage"] = "Código QR generado exitosamente.";
            TempData["QRUrl"] = qrUrl;
            TempData["QRHash"] = qrLink.Hash;
            
            // Decidir a dónde redirigir basado en returnUrl o referer
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            
            // Si viene de la lista (Index), volver a la lista
            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("/Reservas") && !referer.Contains("/Details/"))
            {
                TempData["Success"] = $"Código QR generado para '{reserva.Titulo}'. Accede a los detalles para verlo.";
                return RedirectToAction("Index");
            }
            
            // Por defecto, ir a Details
            return RedirectToAction("Details", new { id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando código QR para reserva {ReservaId}", id);
            TempData["ErrorMessage"] = "Error al generar el código QR.";
            
            // Decidir a dónde redirigir en caso de error
            var referer = Request.Headers["Referer"].ToString();
            if (referer.Contains("/Reservas") && !referer.Contains("/Details/"))
            {
                return RedirectToAction("Index");
            }
            
            return RedirectToAction("Details", new { id = id });
        }
    }

    // GET: /Reservas/DownloadQR/{hash}
    public async Task<IActionResult> DownloadQR(string hash)
    {
        try
        {
            if (string.IsNullOrEmpty(hash))
            {
                return BadRequest("Hash no válido");
            }

            // Validar el QR
            var qrLink = await _qrService.ValidarQRAsync(hash);
            if (qrLink == null)
            {
                return NotFound("Código QR no válido o expirado");
            }

            // Generar la URL completa del QR (apuntando al MVC)
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var qrUrl = $"{baseUrl}/Reservas/QRAccess/{qrLink.Hash}";
            
            // Generar la imagen QR
            var qrImageBase64 = _qrService.GenerarImagenQR(qrUrl);
            var qrImageBytes = Convert.FromBase64String(qrImageBase64);
            
            return File(qrImageBytes, "image/png", $"QR_Reserva_{qrLink.ReservaId}.png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error descargando QR {Hash}", hash);
            return BadRequest("Error al generar la imagen QR");
        }
    }

    // GET: /Reservas/QRAccess/{hash}
    [AllowAnonymous]
    public async Task<IActionResult> QRAccess(string hash)
    {
        try
        {
            if (string.IsNullOrEmpty(hash))
            {
                ViewBag.Error = "Código QR inválido";
                return View("QRError");
            }

            // Validar el QR
            var qrLink = await _qrService.ValidarQRAsync(hash);
            if (qrLink == null)
            {
                ViewBag.Error = "Código QR no válido o expirado";
                return View("QRError");
            }

            // Obtener la reserva
            var reserva = await _context.Reservas
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Id == qrLink.ReservaId);

            if (reserva == null)
            {
                ViewBag.Error = "Reserva no encontrada";
                return View("QRError");
            }

            // Marcar el QR como usado (opcional, dependiendo de la lógica de negocio)
            // await _qrService.MarcarQRComoUsadoAsync(hash);

            return View("QRAccess", reserva);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accediendo via QR {Hash}", hash);
            ViewBag.Error = "Error al acceder via código QR";
            return View("QRError");
        }
    }
} 