
namespace Gestion.Logica.Excepciones;

public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
