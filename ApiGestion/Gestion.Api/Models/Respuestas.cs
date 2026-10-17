using Gestion.Logica.Entidades;

namespace Gestion.Api.Models;

public class UsuarioRespuesta
{
    public int DNI { get; set; }
    public string Nombre { get; set; }
    public string Username { get; set; }
    public string Rol { get; set; }

    public UsuarioRespuesta(Usuario usuario)
    {
        DNI = usuario.DNI;
        Nombre = usuario.Nombre;
        Username = usuario.Username;
        Rol = usuario.Rol.ToString();
    }
}

public class ModalidadRespuesta
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Beneficios { get; set; }
    public decimal Precio { get; set; }
    public int CupoTotal { get; set; }
    public int CupoDisponible { get; set; }

    public ModalidadRespuesta(ModalidadEntrada modalidad)
    {
        Id = modalidad.Id;
        Nombre = modalidad.Nombre;
        Beneficios = modalidad.Beneficios;
        Precio = modalidad.Precio;
        CupoTotal = modalidad.CupoTotal;
        CupoDisponible = modalidad.CupoDisponible;
    }
}

public class EventoRespuesta
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public DateTime Fecha { get; set; }
    public string Lugar { get; set; }
    public bool Cancelado { get; set; }
    public List<ModalidadRespuesta> Modalidades { get; set; }

    public EventoRespuesta(Evento evento)
    {
        Id = evento.Id;
        Nombre = evento.Nombre;
        Descripcion = evento.Descripcion;
        Fecha = evento.ObtenerFechaLocal();
        Lugar = evento.Lugar;
        Cancelado = evento.Cancelado;
        Modalidades = new List<ModalidadRespuesta>();

        foreach (ModalidadEntrada modalidad in evento.Modalidades)
            Modalidades.Add(new ModalidadRespuesta(modalidad));
    }
}

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

public class CompraRespuesta
{
    public int Id { get; set; }
    public int DNIComprador { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Total { get; set; }
    public List<EntradaRespuesta> Entradas { get; set; }

    public CompraRespuesta(Compra compra)
    {
        Id = compra.Id;
        DNIComprador = compra.DNIComprador;
        Fecha = compra.Fecha;
        Total = compra.Total;
        Entradas = new List<EntradaRespuesta>();

        foreach (Entrada entrada in compra.Entradas)
            Entradas.Add(new EntradaRespuesta(entrada));
    }
}

public class ReporteEventoRespuesta
{
    public int EventoId { get; set; }
    public string Nombre { get; set; }
    public int EntradasVendidas { get; set; }
    public decimal Recaudacion { get; set; }

    public ReporteEventoRespuesta(ReporteEvento reporte)
    {
        EventoId = reporte.EventoId;
        Nombre = reporte.Nombre;
        EntradasVendidas = reporte.EntradasVendidas;
        Recaudacion = reporte.Recaudacion;
    }
}
