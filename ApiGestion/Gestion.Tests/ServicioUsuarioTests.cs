using Gestion.Logica.Entidades;
using Gestion.Logica.Servicios;

namespace Gestion.Tests;

public class ServicioUsuarioTests
{
    [TestCase(40123456, RolUsuario.Organizador, false)]
    [TestCase(30111222, RolUsuario.Comprador, false)]
    [TestCase(123, RolUsuario.Organizador, false)]
    [TestCase(30111222, RolUsuario.Organizador, true)]
    [TestCase(40123456, RolUsuario.Comprador, true)]
    public void ValidaPermisosPorDni(int dni, RolUsuario rol, bool permitido)
    {
        var servicio = new ServicioUsuario(new List<Usuario>
        {
            new Usuario { DNI = 30111222, Rol = RolUsuario.Organizador },
            new Usuario { DNI = 40123456, Rol = RolUsuario.Comprador }
        });

        if (permitido)
            Assert.DoesNotThrow(() => servicio.ValidarRol(dni, rol));
        else
            Assert.Throws<UnauthorizedAccessException>(
                () => servicio.ValidarRol(dni, rol));
    }
}
