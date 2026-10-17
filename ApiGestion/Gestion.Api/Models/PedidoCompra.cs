using System.ComponentModel.DataAnnotations;

namespace Gestion.Api.Models;

public class PedidoCompra
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar el DNI.")]
    public int DNI { get; set; }
    public int EventoId { get; set; }
    public int ModalidadId { get; set; }
    public int Cantidad { get; set; }
}
