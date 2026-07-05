using DevTaskManager.Application.DTOs.Dashboard;
using DevTaskManager.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevTaskManager.API.Controllers;

/// <summary>
/// Controlador del Dashboard — expone estadísticas consolidadas del sistema en formato JSON.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Obtiene las estadísticas consolidadas del sistema para el dashboard ejecutivo.
    /// Retorna totales de proyectos, tareas y desarrolladores agrupados por estado/prioridad.
    /// </summary>
    /// <returns>
    /// Objeto JSON con:
    /// - generatedAt: timestamp de generación
    /// - projects: totales por estado (planning, in_progress, on_hold, completed, cancelled)
    /// - tasks: totales por estado y prioridad, completadas este mes, vencidas
    /// - developers: totales por disponibilidad y seniority
    /// </returns>
    [HttpGet("stats")]
    [ProducesResponseType(typeof(DashboardStatsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetStats()
    {
        var stats = await _dashboardService.GetStatsAsync();
        return Ok(stats);
    }
}
