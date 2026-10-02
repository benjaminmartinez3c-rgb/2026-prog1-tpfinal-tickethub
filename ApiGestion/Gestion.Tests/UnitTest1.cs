using Gestion.Logica;
using Gestion.Logica.Archivos;
using Gestion.Logica.Entidades;
using Gestion.Logica.Servicios;

namespace Gestion.Tests;

public class Tests
{
    private Datos _datos = null!;

    [SetUp]
    public void Setup()
    {
        _datos = new Datos();
        var servicio = new ServicioEvento(_datos.Eventos);
        servicio.CrearEvento(new Evento { Nombre = "Recital", Lugar = "Club", Fecha = DateTime.Now.AddDays(1) });
        servicio.AgregarModalidad(1, new ModalidadEntrada { Nombre = "General", Precio = 100, CupoTotal = 10 });
    }

    [Test]
    public void CompraGeneraEntradasIndividualesYDescuentaCupo()
    {
        var compra = new ServicioCompra(_datos).Comprar(1, 1, 3, 40123456);
        Assert.That(compra.Entradas.Count, Is.EqualTo(3));
        Assert.That(compra.Total, Is.EqualTo(300));
        Assert.That(_datos.Eventos[0].Modalidades[0].CupoDisponible, Is.EqualTo(7));
        Assert.That(compra.DNIComprador, Is.EqualTo("40123456"));
    }

    [Test]
    public void CincoEntradasTienenDescuento()
    {
        var compra = new ServicioCompra(_datos).Comprar(1, 1, 5, 40123456);
        Assert.That(compra.Total, Is.EqualTo(450));
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(11)]
    public void RechazaCantidadInvalidaOCupoInsuficiente(int cantidad)
    {
        Assert.Throws<Exception>(() => new ServicioCompra(_datos).Comprar(1, 1, cantidad, 40123456));
        Assert.That(_datos.Compras, Is.Empty);
        Assert.That(_datos.Eventos[0].Modalidades[0].CupoDisponible, Is.EqualTo(10));
    }

    [Test]
    public void RechazaEventoCancelado()
    {
        new ServicioEvento(_datos.Eventos).CancelarEvento(1);
        Assert.Throws<Exception>(() => new ServicioCompra(_datos).Comprar(1, 1, 1, 40123456));
    }

    [Test]
    public void RechazaEventoPasado()
    {
        _datos.Eventos[0].Fecha = DateTime.Now.AddDays(-1);
        Assert.Throws<Exception>(() => new ServicioCompra(_datos).Comprar(1, 1, 1, 40123456));
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

    [TestCase(40123456, "Organizador", false)]
    [TestCase(30111222, "Comprador", false)]
    [TestCase(123, "Organizador", false)]
    [TestCase(30111222, "Organizador", true)]
    [TestCase(40123456, "Comprador", true)]
    public void ValidaPermisosPorDni(int dni, string rol, bool permitido)
    {
        var servicio = new ServicioUsuario(new List<Usuario>
        {
            new Usuario { DNI = 30111222, Rol = "Organizador" },
            new Usuario { DNI = 40123456, Rol = "Comprador" }
        });
        if (permitido) Assert.DoesNotThrow(() => servicio.ValidarRol(dni, rol));
        else Assert.Throws<UnauthorizedAccessException>(() => servicio.ValidarRol(dni, rol));
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
        finally { Directory.Delete(carpeta, true); }
    }
}
