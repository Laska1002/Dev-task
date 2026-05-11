namespace DevTaskManager.Application.Validators;

/// <summary>
/// Validación de cédula ecuatoriana mediante algoritmo módulo 10.
/// Implementado exclusivamente en backend (R1).
/// </summary>
public static class CedulaValidator
{
    /// <summary>
    /// Verifica que una cédula ecuatoriana sea válida según el algoritmo módulo 10.
    /// </summary>
    /// <param name="cedula">Cadena de 10 dígitos.</param>
    /// <returns>true si la cédula es válida.</returns>
    public static bool ValidarCedula(string cedula)
    {
        if (string.IsNullOrEmpty(cedula) || cedula.Length != 10) return false;
        if (!cedula.All(char.IsDigit)) return false;

        int provincia = int.Parse(cedula[..2]);
        if (provincia < 1 || provincia > 24) return false;

        if (int.Parse(cedula[2].ToString()) >= 6) return false;

        int[] coef = { 2, 1, 2, 1, 2, 1, 2, 1, 2 };
        int suma = 0;
        for (int i = 0; i < 9; i++)
        {
            int d = int.Parse(cedula[i].ToString()) * coef[i];
            suma += d >= 10 ? d - 9 : d;
        }

        int verificador = int.Parse(cedula[9].ToString());
        int esperado = suma % 10 == 0 ? 0 : 10 - (suma % 10);
        return verificador == esperado;
    }
}
