namespace Gestion.Logica.Entidades;

public class Entrada
{
    public string Codigo { get; set; } = "";
    public int CompraId { get; set; }
    public int EventoId { get; set; }
    public int ModalidadEntradaId { get; set; }

    public bool Usada { get; set; }
    public DateTime? FechaIngreso { get; set; }
}
