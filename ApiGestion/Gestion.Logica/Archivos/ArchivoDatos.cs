using System.Text.Json;

namespace Gestion.Logica.Archivos;

public class ArchivoDatos
{
    private string _carpeta;
    private JsonSerializerOptions _opciones;

    public ArchivoDatos(string carpeta)
    {
        _carpeta = carpeta;
        Directory.CreateDirectory(carpeta);
        _opciones = new JsonSerializerOptions();
        _opciones.PropertyNameCaseInsensitive = true;
        _opciones.WriteIndented = true;
    }

    public List<Usuario> LeerUsuarios()
    {
        string ruta = Path.Combine(_carpeta, "usuarios.json");
        string json = File.ReadAllText(ruta);
        return JsonSerializer.Deserialize<List<Usuario>>(json, _opciones)!;
    }

    public Datos LeerDatos()
    {
        string ruta = Path.Combine(_carpeta, "datos.json");
        if (!File.Exists(ruta))
            return new Datos();

        string json = File.ReadAllText(ruta);
        return JsonSerializer.Deserialize<Datos>(json, _opciones)!;
    }

    public void GuardarDatos(Datos datos)
    {
        string ruta = Path.Combine(_carpeta, "datos.json");
        string json = JsonSerializer.Serialize(datos, _opciones);
        File.WriteAllText(ruta + ".tmp", json);
        File.Move(ruta + ".tmp", ruta, true);
    }

    public FileStream AbrirBloqueo()
    {
        // Mantener abierto este archivo impide que las dos APIs modifiquen los datos a la vez.
        for (int intento = 0; intento < 100; intento++)
        {
            try
            {
                string ruta = Path.Combine(_carpeta, "datos.lock");
                return new FileStream(ruta, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                Thread.Sleep(20);
            }
        }
        throw new IOException("Los datos están siendo usados. Intentá de nuevo.");
    }
}
