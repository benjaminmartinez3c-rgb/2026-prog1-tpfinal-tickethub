
using Gestion.Logica.Archivos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string carpeta = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "Datos");

builder.Services.AddSingleton(new ArchivoDatos(carpeta));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
