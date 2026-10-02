namespace Gestion.Logica.Entidades;

public class ReporteEvento
{
    public int EventoId { get; set; }
    public string Nombre { get; set; } = "";
    public int EntradasVendidas { get; set; }
    public decimal Recaudacion { get; set; }
}
