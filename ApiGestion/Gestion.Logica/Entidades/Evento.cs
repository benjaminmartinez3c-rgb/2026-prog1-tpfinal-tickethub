namespace Gestion.Logica.Entidades;

public class Evento
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; }
    public bool Cancelado { get; set; }

    public List<ModalidadEntrada> Modalidades { get; set; }

    public Evento()
    {
        Nombre = "";
        Descripcion = "";
        Lugar = "";
        Modalidades = new List<ModalidadEntrada>();
    }

    public DateTime ObtenerFechaLocal()
    {
        if (Fecha.Kind == DateTimeKind.Utc)
            return Fecha.ToLocalTime();

        return Fecha;
    }
}
