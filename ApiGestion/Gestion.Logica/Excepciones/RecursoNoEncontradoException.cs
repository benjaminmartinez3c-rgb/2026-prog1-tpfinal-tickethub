namespace Gestion.Logica.Excepciones;

public class RecursoNoEncontradoException : Exception
{
    public RecursoNoEncontradoException(string mensaje) : base(mensaje)
    {
    }
}
