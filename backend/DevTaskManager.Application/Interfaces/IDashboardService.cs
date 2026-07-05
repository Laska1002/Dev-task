using DevTaskManager.Application.DTOs.Dashboard;

namespace DevTaskManager.Application.Interfaces;

/// <summary>
/// Contrato del servicio de estadísticas del Dashboard.
/// SOLID - DIP: El controlador depende de esta interfaz, no de la implementación concreta.
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Obtiene las estadísticas consolidadas del sistema para el dashboard ejecutivo.
    /// </summary>
    Task<DashboardStatsDto> GetStatsAsync();
}
