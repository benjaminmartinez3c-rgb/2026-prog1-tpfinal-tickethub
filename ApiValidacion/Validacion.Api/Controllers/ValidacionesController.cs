using Microsoft.AspNetCore.Mvc;
using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Gestion.Logica.Excepciones;
using Validacion.Api.Models;
using Validacion.Logica;

namespace Validacion.Api.Controllers;

[ApiController]
[Route("api/validaciones")]
public class ValidacionesController : ControllerBase
{
    private ArchivoDatos _archivos;
    private Datos _datos;
    private ServicioValidacion _servicio;

    public ValidacionesController(
        ArchivoDatos archivos,
        Datos datos,
        ServicioValidacion servicio)
    {
        _archivos = archivos;
        _datos = datos;
        _servicio = servicio;
    }

    [HttpPost]
    public IActionResult Validar(PedidoValidacion pedido)
    {
        try
        {
            Entrada entrada = _servicio.Validar(pedido.Codigo, pedido.EventoId);

            _archivos.GuardarDatos(_datos);
            return Ok(new EntradaRespuesta(entrada));
        }
        catch (RecursoNoEncontradoException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ReglaNegocioException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
