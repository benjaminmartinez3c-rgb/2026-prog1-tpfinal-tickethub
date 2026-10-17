using Microsoft.AspNetCore.Mvc;
using Gestion.Api.Models;
using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Gestion.Logica.Excepciones;
using Gestion.Logica.Servicios;

namespace Gestion.Api.Controllers;

[ApiController]
[Route("api")]
public class GestionController : ControllerBase
{
    private ArchivoDatos _archivos;
    private ServicioUsuario _usuarios;
    private Datos _datos;
    private ServicioEvento _eventos;
    private ServicioCompra _compras;

    public GestionController(
        ArchivoDatos archivos,
        ServicioUsuario usuarios,
        Datos datos,
        ServicioEvento eventos,
        ServicioCompra compras)
    {
        _archivos = archivos;
        _usuarios = usuarios;
        _datos = datos;
        _eventos = eventos;
        _compras = compras;
    }

    [HttpGet("usuarios")]
    public IActionResult ObtenerUsuarios()
    {
        var respuestas = new List<UsuarioRespuesta>();

        foreach (Usuario usuario in _archivos.LeerUsuarios())
            respuestas.Add(new UsuarioRespuesta(usuario));

        return Ok(respuestas);
    }

    [HttpGet("eventos")]
    public IActionResult ObtenerEventos()
    {
        var respuestas = new List<EventoRespuesta>();

        foreach (Evento evento in _eventos.ObtenerEventos())
            respuestas.Add(new EventoRespuesta(evento));

        return Ok(respuestas);
    }

    [HttpGet("eventos/{id}")]
    public IActionResult ObtenerEvento(int id)
    {
        Evento? evento = _eventos.ObtenerEvento(id);

        if (evento == null)
            return NotFound();

        return Ok(new EventoRespuesta(evento));
    }

    [HttpPost("eventos")]
    public IActionResult CrearEvento(int dni, PedidoEvento pedido)
    {
        try
        {
            _usuarios.ValidarRol(dni, RolUsuario.Organizador);

            Evento evento = new Evento
            {
                Nombre = pedido.Nombre,
                Descripcion = pedido.Descripcion,
                Fecha = pedido.Fecha,
                Lugar = pedido.Lugar
            };

            foreach (PedidoModalidad item in pedido.Modalidades)
            {
                evento.Modalidades.Add(new ModalidadEntrada
                {
                    Nombre = item.Nombre,
                    Beneficios = item.Beneficios,
                    Precio = item.Precio,
                    CupoTotal = item.CupoTotal
                });
            }

            Evento creado = _eventos.CrearEvento(evento);

            _archivos.GuardarDatos(_datos);
            return Created("/api/eventos/" + creado.Id, new EventoRespuesta(creado));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
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

    [HttpPut("eventos/{id}")]
    public IActionResult EditarEvento(int id, int dni, PedidoEvento pedido)
    {
        try
        {
            _usuarios.ValidarRol(dni, RolUsuario.Organizador);

            _eventos.EditarEvento(id, pedido.Nombre, pedido.Descripcion, pedido.Fecha, pedido.Lugar);

            _archivos.GuardarDatos(_datos);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
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

    [HttpPut("eventos/{id}/cancelar")]
    public IActionResult CancelarEvento(int id, int dni)
    {
        try
        {
            _usuarios.ValidarRol(dni, RolUsuario.Organizador);

            _eventos.CancelarEvento(id);

            _archivos.GuardarDatos(_datos);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
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

    [HttpPost("eventos/{id}/modalidades")]
    public IActionResult AgregarModalidad(int id, int dni, PedidoModalidad pedido)
    {
        try
        {
            _usuarios.ValidarRol(dni, RolUsuario.Organizador);

            ModalidadEntrada modalidad = new ModalidadEntrada
            {
                Nombre = pedido.Nombre,
                Beneficios = pedido.Beneficios,
                Precio = pedido.Precio,
                CupoTotal = pedido.CupoTotal
            };

            _eventos.AgregarModalidad(id, modalidad);

            _archivos.GuardarDatos(_datos);
            return Ok(new ModalidadRespuesta(modalidad));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
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

    [HttpPost("compras")]
    public IActionResult Comprar(PedidoCompra pedido)
    {
        try
        {
            _usuarios.ValidarRol(pedido.DNI, RolUsuario.Comprador);

            Compra compra = _compras.Comprar(pedido.EventoId, pedido.ModalidadId, pedido.Cantidad, pedido.DNI);

            _archivos.GuardarDatos(_datos);
            return Created("/api/compras/" + compra.Id, new CompraRespuesta(compra));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
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

    [HttpGet("compras/{id}")]
    public IActionResult ObtenerCompra(int id)
    {
        Compra? compra = _compras.ObtenerCompra(id);

        if (compra == null)
            return NotFound();

        return Ok(new CompraRespuesta(compra));
    }

    [HttpGet("reportes/recaudacion")]
    public IActionResult ObtenerRecaudacion(int dni)
    {
        try
        {
            _usuarios.ValidarRol(dni, RolUsuario.Organizador);

            var respuestas = new List<ReporteEventoRespuesta>();

            foreach (ReporteEvento reporte in _compras.ObtenerRecaudacion())
                respuestas.Add(new ReporteEventoRespuesta(reporte));

            return Ok(respuestas);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, ex.Message);
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
