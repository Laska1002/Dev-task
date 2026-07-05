namespace DevTaskManager.Application.DTOs.Dashboard;

/// <summary>
/// DTO principal del endpoint GET /api/dashboard/stats.
/// Consolida estadísticas de proyectos, tareas y desarrolladores en un solo JSON.
/// </summary>
public record DashboardStatsDto(
    DateTime GeneratedAt,
    ProjectStatsDto Projects,
    TaskStatsDto Tasks,
    DeveloperStatsDto Developers
);

public record ProjectStatsDto(
    int Total,
    ProjectByStatusDto ByStatus
);

public record ProjectByStatusDto(
    int Planning,
    int InProgress,
    int OnHold,
    int Completed,
    int Cancelled
);

public record TaskStatsDto(
    int Total,
    TaskByStatusDto ByStatus,
    TaskByPriorityDto ByPriority,
    int CompletedThisMonth,
    int OverdueCount
);

public record TaskByStatusDto(
    int Todo,
    int InProgress,
    int Review,
    int Done,
    int Blocked
);

public record TaskByPriorityDto(
    int Low,
    int Medium,
    int High,
    int Critical
);

public record DeveloperStatsDto(
    int Total,
    int Active,
    DeveloperByAvailabilityDto ByAvailability,
    DeveloperBySeniorityDto BySeniority
);

public record DeveloperByAvailabilityDto(
    int Available,
    int Busy,
    int Vacation
);

public record DeveloperBySeniorityDto(
    int Junior,
    int Mid,
    int Senior
);
