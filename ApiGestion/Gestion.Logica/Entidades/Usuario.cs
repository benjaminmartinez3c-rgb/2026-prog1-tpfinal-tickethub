namespace Gestion.Logica.Entidades;

public class Usuario
{
    public int DNI { get; set; }
    public string Nombre { get; set; }
    public string Username { get; set; }
    public RolUsuario Rol { get; set; }

    public Usuario()
    {
        Nombre = "";
        Username = "";
    }
}
