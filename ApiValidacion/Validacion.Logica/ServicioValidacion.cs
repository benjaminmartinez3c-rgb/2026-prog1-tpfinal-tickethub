using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;

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
            throw new Exception("La entrada no existe.");
        if (entrada.EventoId != eventoId)
            throw new Exception("La entrada es de otro evento.");
        if (entrada.Usada)
            throw new Exception("La entrada ya fue usada.");
        Evento? evento = null;
        foreach (Evento eventoGuardado in _datos.Eventos)
        {
            if (eventoGuardado.Id == eventoId)
                evento = eventoGuardado;
        }
        if (evento == null || evento.Cancelado)
            throw new Exception("El evento no existe o está cancelado.");
        if (evento.Fecha.Date != DateTime.Today)
            throw new Exception("La entrada solo se puede usar el día del evento.");
        entrada.Usada = true;
        entrada.FechaIngreso = DateTime.Now;
        return entrada;
    }
}
