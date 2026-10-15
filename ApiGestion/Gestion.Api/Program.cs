
using Gestion.Logica.Archivos;
using Gestion.Logica.Servicios;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string carpeta = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "Datos");

builder.Services.AddSingleton(new ArchivoDatos(carpeta));
builder.Services.AddTransient<ServicioUsuario>(sp =>
    new ServicioUsuario(sp.GetRequiredService<ArchivoDatos>().LeerUsuarios()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
