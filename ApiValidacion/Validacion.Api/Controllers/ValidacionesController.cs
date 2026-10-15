using Microsoft.AspNetCore.Mvc;
using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Validacion.Api.Models;
using Validacion.Logica;

namespace Validacion.Api.Controllers;

[ApiController]
[Route("api/validaciones")]
public class ValidacionesController : ControllerBase
{
    private ArchivoDatos _archivos;

    
public ValidacionesController(ArchivoDatos archivos)
{
    _archivos = archivos;
}


    [HttpPost]
    public IActionResult Validar(PedidoValidacion pedido)
    {
        try
        {
            using var bloqueo = _archivos.AbrirBloqueo();
            Datos datos = _archivos.LeerDatos();
            var servicio = new ServicioValidacion(datos);
            Entrada entrada = servicio.Validar(pedido.Codigo, pedido.EventoId);
            _archivos.GuardarDatos(datos);
            return Ok(entrada);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
