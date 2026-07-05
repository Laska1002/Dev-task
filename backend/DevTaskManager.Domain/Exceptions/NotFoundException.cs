namespace DevTaskManager.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando un recurso buscado no existe en la base de datos.
/// SOLID - SRP: Esta clase tiene una sola responsabilidad: representar un recurso no encontrado.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string entityName, object id)
        : base($"{entityName} con ID '{id}' no fue encontrado.") { }

    public NotFoundException(string message)
        : base(message) { }
}
