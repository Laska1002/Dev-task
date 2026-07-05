namespace DevTaskManager.Domain.Exceptions;

/// <summary>
/// Excepción lanzada cuando los datos de entrada no pasan las validaciones de negocio.
/// SOLID - SRP: Encapsula todos los errores de validación en un solo tipo.
/// </summary>
public class BusinessValidationException : Exception
{
    public IReadOnlyList<string> Errors { get; }

    public BusinessValidationException(string error)
        : base(error)
    {
        Errors = new List<string> { error };
    }

    public BusinessValidationException(IEnumerable<string> errors)
        : base("Errores de validación de negocio.")
    {
        Errors = errors.ToList();
    }
}
