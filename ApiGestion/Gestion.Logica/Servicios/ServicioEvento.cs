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
        evento.Id = _eventos.Count + 1;


        if (string.IsNullOrWhiteSpace(evento.Nombre) || string.IsNullOrWhiteSpace(evento.Lugar))
            throw new Exception("El nombre y el lugar son obligatorios.");
        if (evento.Fecha <= DateTime.Now)
            throw new Exception("La fecha debe ser futura.");
        var modalidades = evento.Modalidades.ToList();
        evento.Modalidades = new();
        evento.Cancelado = false;

        _eventos.Add(evento);

        foreach (var modalidad in modalidades) AgregarModalidad(evento.Id, modalidad);
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
            if (!evento.Cancelado && evento.Fecha > DateTime.Now)
                disponibles.Add(evento);
        }
        return disponibles;
    }

    public void EditarEvento(int id, string nombre, string descripcion, DateTime fecha, string lugar)
    {
        var evento = ObtenerEvento(id);

        if (evento == null)
            throw new Exception("El evento no existe.");

        if (evento.Cancelado)
            throw new Exception("No se puede editar un evento cancelado.");

        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(lugar) || fecha <= DateTime.Now)
            throw new Exception("El nombre y lugar son obligatorios y la fecha debe ser futura.");
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

        if (string.IsNullOrWhiteSpace(modalidad.Nombre) || modalidad.Precio < 0 || modalidad.CupoTotal <= 0)
            throw new Exception("La modalidad debe tener nombre, precio no negativo y cupo mayor a cero.");
        modalidad.Id = evento.Modalidades.Count + 1;

        modalidad.CupoDisponible = modalidad.CupoTotal;

        evento.Modalidades.Add(modalidad);
    }
}
