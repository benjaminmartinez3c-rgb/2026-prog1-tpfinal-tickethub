using Gestion.Logica.Entidades;

namespace Validacion.Api.Models;

public class EntradaRespuesta
{
    public string Codigo { get; set; }
    public int CompraId { get; set; }
    public int EventoId { get; set; }
    public int ModalidadEntradaId { get; set; }
    public bool Usada { get; set; }
    public DateTime? FechaIngreso { get; set; }

    public EntradaRespuesta(Entrada entrada)
    {
        Codigo = entrada.Codigo;
        CompraId = entrada.CompraId;
        EventoId = entrada.EventoId;
        ModalidadEntradaId = entrada.ModalidadEntradaId;
        Usada = entrada.Usada;
        FechaIngreso = entrada.FechaIngreso;
    }
}
