using Catalogo_Backend_.Data;
using Microsoft.EntityFrameworkCore;
using Catalogo_Backend_.Models;

var builder = WebApplication.CreateBuilder(args);

//* Se lee la cadena de conexión desde el archivo de configuración (appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//* Se registra el DbContext con PostgreSQL (Npgsql)
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build(); //* Construye la aplicación

//* Se crea el scope para obtener las instancias de los servicios
using var scope = app.Services.CreateScope();
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();

//? Probando conexión a base de datos
if (!ctx.Database.CanConnect())
{
    logger.LogError("No se pudo conectar a la base de datos");
    //throw new Exception("No se pudo conectar a la base de datos");
}
else
{
    //Console.WriteLine("Conexión a la base de datos exítosa");
    logger.LogInformation("Conexión a la base de datos exítosa");
}

//? Sembrando datos de prueba
ctx.Users.Add(new User
{
    Id = Guid.NewGuid(),
    Name = "Héctor Jesús",
    Username = "Blxsh",
    Email = "hectorjesus029@gmail.com",
    PasswordHash = "ABC2002",
    Role = "admin",
    IsActive = true
});
ctx.SaveChanges();

app.Run();