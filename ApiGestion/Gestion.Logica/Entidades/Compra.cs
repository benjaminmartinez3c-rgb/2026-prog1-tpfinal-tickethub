namespace Gestion.Logica.Entidades;

public class Compra
{
    public int Id { get; set; }
    public string DNIComprador { get; set; } = "";
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }

    public List<Entrada> Entradas { get; set; } = new();
}
