using Gestion.Logica.Entidades;

namespace Gestion.Logica.Archivos;

public class Datos
{
    public List<Evento> Eventos { get; set; }
    public List<Compra> Compras { get; set; }

    public Datos()
    {
        Eventos = new List<Evento>();
        Compras = new List<Compra>();
    }
}
