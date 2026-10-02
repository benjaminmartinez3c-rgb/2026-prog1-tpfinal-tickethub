using Microsoft.AspNetCore.Mvc;
using Gestion.Api.Models;
using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Gestion.Logica.Servicios;

namespace Gestion.Api.Controllers;

[ApiController]
[Route("api")]
public class GestionController : ControllerBase
{
    private ArchivoDatos _archivos;
    private ServicioUsuario _usuarios;

    public GestionController(IWebHostEnvironment entorno)
    {
        string carpeta = Path.Combine(entorno.ContentRootPath, "..", "..", "Datos");
        _archivos = new ArchivoDatos(carpeta);
        _usuarios = new ServicioUsuario(_archivos.LeerUsuarios());
    }

    [HttpGet("usuarios")]
    public IActionResult ObtenerUsuarios()
    {
        return Ok(_archivos.LeerUsuarios());
    }

    [HttpGet("eventos")]
    public IActionResult ObtenerEventos()
    {
        using var bloqueo = _archivos.AbrirBloqueo();
        Datos datos = _archivos.LeerDatos();
        var servicio = new ServicioEvento(datos.Eventos);
        return Ok(servicio.ObtenerEventos());
    }

    [HttpGet("eventos/{id}")]
    public IActionResult ObtenerEvento(int id)
    {
        using var bloqueo = _archivos.AbrirBloqueo();
        Datos datos = _archivos.LeerDatos();
        var servicio = new ServicioEvento(datos.Eventos);
        Evento? evento = servicio.ObtenerEvento(id);
        if (evento == null)
            return NotFound();
        return Ok(evento);
    }
    [HttpPost("eventos")]
    public IActionResult CrearEvento(int dni, Evento evento)
    {
        try
        {
            _usuarios.ValidarRol(dni, "Organizador");
            using var bloqueo = _archivos.AbrirBloqueo();
            Datos datos = _archivos.LeerDatos();
            var servicio = new ServicioEvento(datos.Eventos);
            Evento creado = servicio.CrearEvento(evento);
            _archivos.GuardarDatos(datos);
            return Created("/api/eventos/" + creado.Id, creado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("eventos/{id}")]
    public IActionResult EditarEvento(int id, int dni, Evento evento)
    {
        try
        {
            _usuarios.ValidarRol(dni, "Organizador");
            using var bloqueo = _archivos.AbrirBloqueo();
            Datos datos = _archivos.LeerDatos();
            var servicio = new ServicioEvento(datos.Eventos);
            servicio.EditarEvento(id, evento.Nombre, evento.Descripcion, evento.Fecha, evento.Lugar);
            _archivos.GuardarDatos(datos);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("eventos/{id}/cancelar")]
    public IActionResult CancelarEvento(int id, int dni)
    {
        try
        {
            _usuarios.ValidarRol(dni, "Organizador");
            using var bloqueo = _archivos.AbrirBloqueo();
            Datos datos = _archivos.LeerDatos();
            var servicio = new ServicioEvento(datos.Eventos);
            servicio.CancelarEvento(id);
            _archivos.GuardarDatos(datos);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("eventos/{id}/modalidades")]
    public IActionResult AgregarModalidad(int id, int dni, ModalidadEntrada modalidad)
    {
        try
        {
            _usuarios.ValidarRol(dni, "Organizador");
            using var bloqueo = _archivos.AbrirBloqueo();
            Datos datos = _archivos.LeerDatos();
            var servicio = new ServicioEvento(datos.Eventos);
            servicio.AgregarModalidad(id, modalidad);
            _archivos.GuardarDatos(datos);
            return Ok(modalidad);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("compras")]
    public IActionResult Comprar(PedidoCompra pedido)
    {
        try
        {
            _usuarios.ValidarRol(pedido.DNI, "Comprador");
            using var bloqueo = _archivos.AbrirBloqueo();
            Datos datos = _archivos.LeerDatos();
            var servicio = new ServicioCompra(datos);
            Compra compra = servicio.Comprar(pedido.EventoId, pedido.ModalidadId, pedido.Cantidad, pedido.DNI);
            _archivos.GuardarDatos(datos);
            return Created("/api/compras/" + compra.Id, compra);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpGet("compras/{id}")]
    public IActionResult ObtenerCompra(int id)
    {
        using var bloqueo = _archivos.AbrirBloqueo();
        Datos datos = _archivos.LeerDatos();
        var servicio = new ServicioCompra(datos);
        Compra? compra = servicio.ObtenerCompra(id);
        if (compra == null)
            return NotFound();
        return Ok(compra);
    }
    [HttpGet("reportes/recaudacion")]
    public IActionResult ObtenerRecaudacion(int dni)
    {
        try
        {
            _usuarios.ValidarRol(dni, "Organizador");
            using var bloqueo = _archivos.AbrirBloqueo();
            Datos datos = _archivos.LeerDatos();
            var servicio = new ServicioCompra(datos);
            return Ok(servicio.ObtenerRecaudacion());
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

