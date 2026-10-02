var builder = WebApplication.CreateBuilder(args);

// Registra los controladores y Swagger.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Publica las rutas escritas en los controladores.
app.MapControllers();
app.Run();
