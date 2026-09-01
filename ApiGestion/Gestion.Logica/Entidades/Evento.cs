namespace Gestion.Logica.Entidades;

public class Evento
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Descripcion { get; set; } = "";
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; } = "";
    public bool Cancelado { get; set; }

    public List<ModalidadEntrada> Modalidades { get; set; } = new();
}
