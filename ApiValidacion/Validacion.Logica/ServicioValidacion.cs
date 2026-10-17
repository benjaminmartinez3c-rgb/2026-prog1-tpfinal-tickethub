using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Gestion.Logica.Excepciones;

namespace Validacion.Logica;

public class ServicioValidacion
{
    private readonly Datos _datos;

    public ServicioValidacion(Datos datos)
    {
        _datos = datos;
    }

    public Entrada Validar(string codigo, int eventoId)
    {
        if (string.IsNullOrWhiteSpace(codigo) || eventoId <= 0)
            throw new ReglaNegocioException("Debe indicar el código y el evento.");

        codigo = codigo.Trim().ToUpper();

        Entrada? entrada = null;

        foreach (Compra compra in _datos.Compras)
        {
            foreach (Entrada entradaCompra in compra.Entradas)
            {
                if (entradaCompra.Codigo == codigo)
                    entrada = entradaCompra;
            }
        }

        if (entrada == null)
            throw new RecursoNoEncontradoException("La entrada no existe.");

        if (entrada.EventoId != eventoId)
            throw new ReglaNegocioException("La entrada es de otro evento.");

        if (entrada.Usada)
            throw new ReglaNegocioException("La entrada ya fue usada.");

        Evento? evento = null;

        foreach (Evento eventoGuardado in _datos.Eventos)
        {
            if (eventoGuardado.Id == eventoId)
                evento = eventoGuardado;
        }

        if (evento == null)
            throw new RecursoNoEncontradoException("El evento no existe.");

        if (evento.Cancelado)
            throw new ReglaNegocioException("El evento está cancelado.");

        if (evento.ObtenerFechaLocal().Date != DateTime.Today)
            throw new ReglaNegocioException("La entrada solo se puede usar el día del evento.");

        entrada.Usada = true;
        entrada.FechaIngreso = DateTime.Now;

        return entrada;
    }
}
