using Gestion.Logica.Entidades;
using Gestion.Logica.Excepciones;

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
        evento.Fecha = evento.ObtenerFechaLocal();
        evento.Id = _eventos.Count + 1;

        if (string.IsNullOrWhiteSpace(evento.Nombre) || string.IsNullOrWhiteSpace(evento.Lugar))
            throw new ReglaNegocioException("El nombre y el lugar son obligatorios.");

        if (evento.Fecha <= DateTime.Now)
            throw new ReglaNegocioException("La fecha debe ser futura.");

        var modalidades = evento.Modalidades;
        evento.Modalidades = new();
        evento.Cancelado = false;

        _eventos.Add(evento);

        foreach (ModalidadEntrada modalidad in modalidades)
            AgregarModalidad(evento.Id, modalidad);

        return evento;
    }

    public Evento? ObtenerEvento(int id)
    {
        foreach (Evento evento in _eventos)
        {
            if (evento.Id == id)
                return evento;
        }

        return null;
    }

    public List<Evento> ObtenerEventos()
    {
        var disponibles = new List<Evento>();

        foreach (Evento evento in _eventos)
        {
            if (!evento.Cancelado && evento.ObtenerFechaLocal() > DateTime.Now)
                disponibles.Add(evento);
        }

        return disponibles;
    }

    public void EditarEvento(int id, string nombre, string descripcion, DateTime fecha, string lugar)
    {
        var evento = ObtenerEvento(id);

        if (evento == null)
            throw new RecursoNoEncontradoException("El evento no existe.");

        if (evento.Cancelado)
            throw new ReglaNegocioException("No se puede editar un evento cancelado.");

        if (fecha.Kind == DateTimeKind.Utc)
            fecha = fecha.ToLocalTime();

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(lugar) || fecha <= DateTime.Now)
            throw new ReglaNegocioException("El nombre y lugar son obligatorios y la fecha debe ser futura.");

        evento.Nombre = nombre;
        evento.Descripcion = descripcion;
        evento.Fecha = fecha;
        evento.Lugar = lugar;
    }

    public void CancelarEvento(int id)
    {
        var evento = ObtenerEvento(id);

        if (evento == null)
            throw new RecursoNoEncontradoException("El evento no existe.");

        if (evento.Cancelado)
            throw new ReglaNegocioException("El evento ya está cancelado.");

        evento.Cancelado = true;
    }

    public void AgregarModalidad(int eventoId, ModalidadEntrada modalidad)
    {
        var evento = ObtenerEvento(eventoId);

        if (evento == null)
            throw new RecursoNoEncontradoException("El evento no existe.");

        if (evento.Cancelado)
            throw new ReglaNegocioException("No se puede agregar una modalidad a un evento cancelado.");

        if (string.IsNullOrWhiteSpace(modalidad.Nombre) || modalidad.Precio < 0 || modalidad.CupoTotal <= 0)
            throw new ReglaNegocioException("La modalidad debe tener nombre, precio no negativo y cupo mayor a cero.");

        modalidad.Id = evento.Modalidades.Count + 1;
        modalidad.CupoDisponible = modalidad.CupoTotal;

        evento.Modalidades.Add(modalidad);
    }
}
