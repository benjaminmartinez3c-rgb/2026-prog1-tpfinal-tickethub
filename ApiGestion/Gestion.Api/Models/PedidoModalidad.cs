
namespace Gestion.Api.Models;

public class PedidoModalidad
{
    public string Nombre { get; set; }
    public string Beneficios { get; set; }
    public decimal Precio { get; set; }
    public int CupoTotal { get; set; }

    public PedidoModalidad()
    {
        Nombre = "";
        Beneficios = "";
    }
}
