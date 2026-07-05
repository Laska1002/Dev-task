namespace DevTaskManager.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando las credenciales o permisos son inválidos.
/// SOLID - SRP: Un solo tipo de error = una sola clase.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}
