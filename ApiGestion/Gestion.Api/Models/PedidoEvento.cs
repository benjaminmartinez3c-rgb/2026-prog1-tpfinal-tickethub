
namespace Gestion.Api.Models;

public class PedidoEvento
{
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = "";
    public List<PedidoModalidad> Modalidades { get; set; } = new();
}
