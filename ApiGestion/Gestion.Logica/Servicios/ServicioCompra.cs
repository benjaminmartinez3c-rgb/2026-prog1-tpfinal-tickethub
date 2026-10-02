using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;

namespace Gestion.Logica.Servicios;

public class ServicioCompra
{
    private readonly Datos _datos;

    public ServicioCompra(Datos datos)
    {
        _datos = datos;
    }

    public Compra Comprar(int eventoId, int modalidadId, int cantidad, int dni)
    {
        Evento? evento = new ServicioEvento(_datos.Eventos).ObtenerEvento(eventoId);
        if (evento == null)
            throw new Exception("El evento no existe.");
        if (evento.Cancelado || evento.Fecha <= DateTime.Now)
            throw new Exception("No se puede comprar para un evento cancelado o cuya fecha ya pasó.");
        ModalidadEntrada? modalidad = null;
        foreach (ModalidadEntrada modalidadEvento in evento.Modalidades)
        {
            if (modalidadEvento.Id == modalidadId)
                modalidad = modalidadEvento;
        }
        if (modalidad == null)
            throw new Exception("La modalidad no existe.");
        if (cantidad <= 0)
            throw new Exception("La cantidad debe ser mayor a cero.");
        if (cantidad > modalidad.CupoDisponible)
            throw new Exception("No queda cupo suficiente.");
        // Seis caracteres en base 36. El contador se guarda y nunca se reutiliza.
        if (_datos.UltimoCodigo + cantidad >= 2176782336L)
            throw new Exception("No quedan códigos disponibles.");

        var compra = new Compra
        {
            Id = _datos.Compras.Count + 1,
            DNIComprador = dni.ToString(),
            Fecha = DateTime.Now,
            Total = modalidad.Precio * cantidad
        };
        if (cantidad >= 5)
            compra.Total = compra.Total * 0.9m;

        for (int i = 0; i < cantidad; i++)
        {
            _datos.UltimoCodigo++;
            compra.Entradas.Add(new Entrada
            {
                Codigo = GenerarCodigo(_datos.UltimoCodigo),
                CompraId = compra.Id,
                EventoId = eventoId,
                ModalidadEntradaId = modalidadId
            });
        }
        modalidad.CupoDisponible -= cantidad;
        _datos.Compras.Add(compra);
        return compra;
    }

    private string GenerarCodigo(long numero)
    {
        const string caracteres = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var codigo = new char[6];
        for (int i = 5; i >= 0; i--)
        {
            codigo[i] = caracteres[(int)(numero % 36)];
            numero /= 36;
        }
        return new string(codigo);
    }

    public Compra? ObtenerCompra(int id)
    {
        foreach (Compra compra in _datos.Compras)
        {
            if (compra.Id == id)
                return compra;
        }
        return null;
    }

    public List<ReporteEvento> ObtenerRecaudacion()
    {
        var reportes = new List<ReporteEvento>();
        foreach (Evento evento in _datos.Eventos)
        {
            var reporte = new ReporteEvento();
            reporte.EventoId = evento.Id;
            reporte.Nombre = evento.Nombre;
            foreach (Compra compra in _datos.Compras)
            {
                bool perteneceAlEvento = false;
                foreach (Entrada entrada in compra.Entradas)
                {
                    if (entrada.EventoId == evento.Id)
                    {
                        reporte.EntradasVendidas++;
                        perteneceAlEvento = true;
                    }
                }
                if (perteneceAlEvento)
                    reporte.Recaudacion += compra.Total;
            }
            reportes.Add(reporte);
        }
        return reportes;
    }
}
