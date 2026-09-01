using Gestion.Logica.Entidades;

namespace Gestion.Logica.Servicios;

public class ServicioEvento
{
    private readonly List<Evento> _eventos;

    public ServicioEvento(List<Evento> eventos)
    {
        _eventos = eventos;
    }

    public Evento CrearEvento(Evento evento)
    {
        if (_eventos.Count == 0)
    {
        evento.Id = 1;
    }
    else
    {
        evento.Id = _eventos.Max(e => e.Id) + 1;
    }


        evento.Cancelado = false;

        _eventos.Add(evento);

        return evento;
    }

    public Evento? ObtenerEvento(int id)
    {
        return _eventos.FirstOrDefault(e => e.Id == id);
    }

    public List<Evento> ObtenerEventos()
    {
        return _eventos;
    }

    public void EditarEvento(int id, string nombre, string descripcion, DateTime fecha, string lugar)
    {
        var evento = ObtenerEvento(id);

        if (evento == null)
            throw new Exception("El evento no existe.");

        if (evento.Cancelado)
            throw new Exception("No se puede editar un evento cancelado.");

        evento.Nombre = nombre;
        evento.Descripcion = descripcion;
        evento.Fecha = fecha;
        evento.Lugar = lugar;
    }

    public void CancelarEvento(int id)
    {
        var evento = ObtenerEvento(id);

        if (evento == null)
            throw new Exception("El evento no existe.");

        if (evento.Cancelado)
            throw new Exception("El evento ya está cancelado.");

        evento.Cancelado = true;
    }

    public void AgregarModalidad(int eventoId, ModalidadEntrada modalidad)
    {
        var evento = ObtenerEvento(eventoId);

        if (evento == null)
            throw new Exception("El evento no existe.");

        if (evento.Cancelado)
            throw new Exception("No se puede agregar una modalidad a un evento cancelado.");

        if (evento.Modalidades.Count == 0)
        {
            modalidad.Id = 1;
        } else
        {
            modalidad.Id = evento.Modalidades.Max(m => m.Id) + 1;
        }

        modalidad.CupoDisponible = modalidad.CupoTotal;

        evento.Modalidades.Add(modalidad);
    }
}
