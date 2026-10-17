using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Gestion.Logica.Excepciones;
using Gestion.Logica.Servicios;

namespace Gestion.Tests;

public class ServicioCompraTests
{
    private Datos _datos = null!;

    [SetUp]
    public void Setup()
    {
        _datos = new Datos();

        var servicio = new ServicioEvento(_datos.Eventos);
        servicio.CrearEvento(new Evento
        {
            Nombre = "Recital",
            Lugar = "Club",
            Fecha = DateTime.Now.AddDays(1)
        });

        servicio.AgregarModalidad(1, new ModalidadEntrada
        {
            Nombre = "General",
            Precio = 100,
            CupoTotal = 10
        });
    }

    [Test]
    public void CompraGeneraEntradasIndividualesYDescuentaCupo()
    {
        var compra = new ServicioCompra(_datos).Comprar(1, 1, 3, 40123456);

        Assert.That(compra.Entradas.Count, Is.EqualTo(3));
        Assert.That(compra.Total, Is.EqualTo(300));
        Assert.That(_datos.Eventos[0].Modalidades[0].CupoDisponible, Is.EqualTo(7));
        Assert.That(compra.DNIComprador, Is.EqualTo(40123456));
    }

    [Test]
    public void CincoEntradasTienenDescuento()
    {
        var compra = new ServicioCompra(_datos).Comprar(1, 1, 5, 40123456);

        Assert.That(compra.Total, Is.EqualTo(450));
    }

    [Test]
    public void CuatroEntradasNoTienenDescuento()
    {
        // Arrange
        var servicio = new ServicioCompra(_datos);

        // Act
        var compra = servicio.Comprar(1, 1, 4, 40123456);

        // Assert
        Assert.That(compra.Total, Is.EqualTo(400));
    }

    [Test]
    public void CodigosNoSeRepitenEntreCompras()
    {
        // Arrange
        var servicio = new ServicioCompra(_datos);
        var codigos = new List<string>();

        // Act
        servicio.Comprar(1, 1, 5, 40123456);
        servicio.Comprar(1, 1, 5, 38456789);

        // Assert
        foreach (Compra compra in _datos.Compras)
        {
            foreach (Entrada entrada in compra.Entradas)
            {
                Assert.That(codigos.Contains(entrada.Codigo), Is.False);
                codigos.Add(entrada.Codigo);
            }
        }

        Assert.That(codigos.Count, Is.EqualTo(10));
    }

    [Test]
    public void EventoInexistenteLanzaExcepcionCorrecta()
    {
        // Arrange
        var servicio = new ServicioCompra(_datos);

        // Act y Assert
        Assert.Throws<RecursoNoEncontradoException>(
            () => servicio.Comprar(999, 1, 1, 40123456));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(11)]
    public void RechazaCantidadInvalidaOCupoInsuficiente(int cantidad)
    {
        Assert.Throws<ReglaNegocioException>(
            () => new ServicioCompra(_datos).Comprar(1, 1, cantidad, 40123456));

        Assert.That(_datos.Compras, Is.Empty);
        Assert.That(_datos.Eventos[0].Modalidades[0].CupoDisponible, Is.EqualTo(10));
    }

    [Test]
    public void RechazaEventoCancelado()
    {
        new ServicioEvento(_datos.Eventos).CancelarEvento(1);

        Assert.Throws<ReglaNegocioException>(
            () => new ServicioCompra(_datos).Comprar(1, 1, 1, 40123456));
    }

    [Test]
    public void RechazaEventoPasado()
    {
        _datos.Eventos[0].Fecha = DateTime.Now.AddDays(-1);

        Assert.Throws<ReglaNegocioException>(
            () => new ServicioCompra(_datos).Comprar(1, 1, 1, 40123456));
    }

    [Test]
    public void ModalidadesTienenCuposYPreciosIndependientes()
    {
        new ServicioEvento(_datos.Eventos).AgregarModalidad(1,
            new ModalidadEntrada { Nombre = "VIP", Precio = 200, CupoTotal = 2 });

        var compra = new ServicioCompra(_datos).Comprar(1, 2, 1, 40123456);

        Assert.That(compra.Total, Is.EqualTo(200));
        Assert.That(_datos.Eventos[0].Modalidades[0].CupoDisponible, Is.EqualTo(10));
        Assert.That(_datos.Eventos[0].Modalidades[1].CupoDisponible, Is.EqualTo(1));
    }

    [Test]
    public void PersistenciaConservaDatosYCodigosEntreInstancias()
    {
        string carpeta = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        try
        {
            var archivo = new ArchivoDatos(carpeta);
            new ServicioCompra(_datos).Comprar(1, 1, 5, 40123456);
            archivo.GuardarDatos(_datos);

            var otroArchivo = new ArchivoDatos(carpeta);
            Datos datos = otroArchivo.LeerDatos();
            new ServicioCompra(datos).Comprar(1, 1, 5, 40123456);
            otroArchivo.GuardarDatos(datos);

            datos = archivo.LeerDatos();
            var codigos = new List<string>();
            decimal total = 0;

            foreach (Compra compra in datos.Compras)
            {
                total += compra.Total;

                foreach (Entrada entrada in compra.Entradas)
                {
                    Assert.That(codigos.Contains(entrada.Codigo), Is.False);
                    Assert.That(entrada.Codigo.Length, Is.EqualTo(6));

                    foreach (char caracter in entrada.Codigo)
                        Assert.That(char.IsLetterOrDigit(caracter), Is.True);

                    codigos.Add(entrada.Codigo);
                }
            }

            Assert.That(codigos.Count, Is.EqualTo(10));
            Assert.That(datos.Eventos[0].Modalidades[0].CupoDisponible, Is.Zero);
            Assert.That(total, Is.EqualTo(900));
        }
        finally
        {
            Directory.Delete(carpeta, true);
        }
    }

    [Test]
    public void RechazaCompraParaEventoPasadoExpresadoEnUtc()
    {
        _datos.Eventos[0].Fecha = DateTime.Now.AddHours(-1).ToUniversalTime();
        var servicio = new ServicioCompra(_datos);

        Assert.Throws<ReglaNegocioException>(
            () => servicio.Comprar(1, 1, 1, 40123456));

        Assert.That(_datos.Compras, Is.Empty);
    }

    [Test]
    public void RechazaCrearEventoPasadoExpresadoEnUtc()
    {
        var servicio = new ServicioEvento(_datos.Eventos);
        var evento = new Evento
        {
            Nombre = "Evento pasado",
            Lugar = "Club",
            Fecha = DateTime.Now.AddHours(-1).ToUniversalTime()
        };

        Assert.Throws<ReglaNegocioException>(
            () => servicio.CrearEvento(evento));
    }
}
