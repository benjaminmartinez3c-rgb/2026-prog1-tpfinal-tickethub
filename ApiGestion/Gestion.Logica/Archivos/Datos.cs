using Gestion.Logica.Entidades;

namespace Gestion.Logica.Archivos;

public class Datos
{
    public List<Evento> Eventos { get; set; } = new();
    public List<Compra> Compras { get; set; } = new();
    public long UltimoCodigo { get; set; }
}
