
using Gestion.Logica.Entidades;

namespace Gestion.Logica.Servicios;

public class ServicioUsuario
{
    private readonly List<Usuario> _usuarios;

    public ServicioUsuario(List<Usuario> usuarios)
    {
        _usuarios = usuarios;
    }

    public void ValidarRol(int dni, string rol)
    {
        Usuario? usuario = null;

        foreach (Usuario usuarioGuardado in _usuarios)
        {
            if (usuarioGuardado.DNI == dni)
                usuario = usuarioGuardado;
        }

        if (usuario == null || usuario.Rol != rol)
            throw new UnauthorizedAccessException("el usuario no tiene permiso para esta accion");
    }
}
