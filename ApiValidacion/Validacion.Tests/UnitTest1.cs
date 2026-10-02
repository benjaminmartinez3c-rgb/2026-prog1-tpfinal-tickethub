using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Validacion.Logica;

namespace Validacion.Tests;

public class Tests
{
    private Datos _datos = null!;

    [SetUp]
    public void Setup()
    {
        _datos = new Datos
        {
            Eventos = new() { new Evento { Id = 1, Fecha = DateTime.Today } },
            Compras = new()
            {
                new Compra { Id = 1, Entradas = new()
                {
                    new Entrada { Codigo = "000001", EventoId = 1 },
                    new Entrada { Codigo = "000002", EventoId = 1 }
                } }
            }
        };
    }

    [Test]
    public void ValidaSoloLaEntradaIndicada()
    {
        var entrada = new ServicioValidacion(_datos).Validar("000001", 1);
        Assert.That(entrada.Usada, Is.True);
        Assert.That(entrada.FechaIngreso, Is.Not.Null);
        Assert.That(_datos.Compras[0].Entradas[1].Usada, Is.False);
    }

    [TestCase("XXXXXX", 1, "La entrada no existe.")]
    [TestCase("000001", 2, "La entrada es de otro evento.")]
    public void RechazaEntradaInexistenteODeOtroEvento(string codigo, int eventoId, string mensaje)
    {
        var error = Assert.Throws<Exception>(() => new ServicioValidacion(_datos).Validar(codigo, eventoId));
        Assert.That(error!.Message, Is.EqualTo(mensaje));
        Assert.That(_datos.Compras[0].Entradas[0].Usada, Is.False);
    }

    [Test]
    public void RechazaSegundoIngreso()
    {
        var servicio = new ServicioValidacion(_datos);
        servicio.Validar("000001", 1);
        var error = Assert.Throws<Exception>(() => servicio.Validar("000001", 1));
        Assert.That(error!.Message, Is.EqualTo("La entrada ya fue usada."));
    }

    [TestCase(-1)]
    [TestCase(1)]
    public void RechazaFueraDelDiaDelEvento(int dias)
    {
        _datos.Eventos[0].Fecha = DateTime.Today.AddDays(dias);
        Assert.Throws<Exception>(() => new ServicioValidacion(_datos).Validar("000001", 1));
    }

    [Test]
    public void RechazaEventoCancelado()
    {
        _datos.Eventos[0].Cancelado = true;
        Assert.Throws<Exception>(() => new ServicioValidacion(_datos).Validar("000001", 1));
    }

    [Test]
    public void IngresoPersisteParaOtraInstancia()
    {
        string carpeta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        try
        {
            var archivo = new ArchivoDatos(carpeta);
            archivo.GuardarDatos(_datos);
            Datos datos = archivo.LeerDatos();
            new ServicioValidacion(datos).Validar("000001", 1);
            archivo.GuardarDatos(datos);

            var otroArchivo = new ArchivoDatos(carpeta);
            Datos datosLeidos = otroArchivo.LeerDatos();
            var servicio = new ServicioValidacion(datosLeidos);
            Assert.Throws<Exception>(() => servicio.Validar("000001", 1));
            Assert.That(datosLeidos.Compras[0].Entradas[0].Usada, Is.True);
        }
        finally { Directory.Delete(carpeta, true); }
    }
}
