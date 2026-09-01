namespace Gestion.Logica.Entidades;

public class ModalidadEntrada
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Beneficios { get; set; } = "";
    public decimal Precio { get; set; }
    public int CupoTotal { get; set; }
    public int CupoDisponible { get; set; }
}
