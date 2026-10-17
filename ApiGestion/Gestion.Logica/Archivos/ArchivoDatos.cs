using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Gestion.Logica.Entidades;

namespace Gestion.Logica.Archivos;

public class ArchivoDatos
{
    private string _carpeta;
    private JsonSerializerSettings _opciones;

    public ArchivoDatos(string carpeta)
    {
        _carpeta = carpeta;
        Directory.CreateDirectory(carpeta);

        _opciones = new JsonSerializerSettings();
        _opciones.Formatting = Formatting.Indented;
        _opciones.Converters.Add(new StringEnumConverter());
    }

    public List<Usuario> LeerUsuarios()
    {
        string ruta = Path.Combine(_carpeta, "usuarios.json");
        string json = File.ReadAllText(ruta);
        return JsonConvert.DeserializeObject<List<Usuario>>(json, _opciones)!;
    }

    public Datos LeerDatos()
    {
        string ruta = Path.Combine(_carpeta, "datos.json");

        if (!File.Exists(ruta))
            return new Datos();

        string json = File.ReadAllText(ruta);
        return JsonConvert.DeserializeObject<Datos>(json, _opciones)!;
    }

    public void GuardarDatos(Datos datos)
    {
        string ruta = Path.Combine(_carpeta, "datos.json");
        string json = JsonConvert.SerializeObject(datos, _opciones);

        File.WriteAllText(ruta + ".tmp", json);
        File.Move(ruta + ".tmp", ruta, true);
    }
}
