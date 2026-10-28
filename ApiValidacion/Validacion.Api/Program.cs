using Gestion.Logica.Archivos;
using Validacion.Logica;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

string carpeta = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "Datos");

builder.Services.AddSingleton(new ArchivoDatos(carpeta));

builder.Services.AddScoped<FileStream>(sp =>
    sp.GetRequiredService<ArchivoDatos>().AbrirBloqueo());

builder.Services.AddScoped<Datos>(sp =>
{
    sp.GetRequiredService<FileStream>();

    return sp.GetRequiredService<ArchivoDatos>().LeerDatos();
});

builder.Services.AddScoped<ServicioValidacion>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.MapControllers();
app.Run();
