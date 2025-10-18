using CatalogoBackend.Data;
using CatalogoBackend.Data.DAOs;
using CatalogoBackend.Data.IA;
using CatalogoBackend.Models.Entities;
using CatalogoBackend.Services;
using CatalogoBackend.Services.IA;
using CatalogoBackend.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//* Se lee la cadena de conexión desde el archivo de configuración (appsettings.json)
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
//* Variable de tipo de permiso de las CORS
var CorsPolicyOrigins = "_CorsPolicyOrigins";
//* Variable para el uso de las propiedades de la configuración de Jwt
var JwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

//~ EXPLICACIÓN DE CONSOLA Y NIVELES DE LOGGING
builder.Logging.ClearProviders(); //* Limpia los proveedores de logging por defecto
builder.Logging.AddConsole(); //* Agrega el proveedor de logging para consola
builder.Logging.AddDebug(); //* Se añade el debug logger que envía logs al output -> debug
builder.Logging.AddFilter("Microsoft", LogLevel.Warning); //* Filtra los logs de Microsoft a Warning
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning); //* Filtra los logs de Entity Framework Core a Warning
builder.Logging.AddFilter("CatalogoBackend.Controllers.UsersController", LogLevel.Information); //* Filtra los logs del controlador de usuarios a Information
builder.Logging.AddFilter("CatalogoBackend.Controllers.ProductsController", LogLevel.Information);
builder.Logging.AddFilter("CatalogoBackend.Services.EmailService", LogLevel.Information);
builder.Logging.AddFilter("CatalogoBackend.Services.UserService", LogLevel.Information);

//* Se registra el DbContext con PostgreSQL (Npgsql)
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(ConnectionString));

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddControllers();
builder.Services.AddScoped<IUserDao, UserDao>().AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenDao, TokenDao>().AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IProductDao, ProductDao>().AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailGenerator>();
builder.Services.AddScoped<JwtGenerator>();
builder.Services.AddHostedService<TokenCleanupService>();

//? Se crea y registra las politicas de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });  
});

//? Se configura la manera en el que se va a autenticar o validar el JWT que llegue en cada petición
builder.Services.AddAuthentication(options => {

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; //* El que asigna como validar al usuario.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; //* El que asigna que hacer cuando el usuario no tenga un Bearer válido.

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = JwtSettings.Issuer,
        ValidAudience = JwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSettings.Key)),
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

// 🚀 Se añade el Swagger al contenedor de dependencias
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Catálogo de productos", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "Catálogo de productos", Version = "v2" });

    // 🔐 Configuración de seguridad (JWT) para saber como es que debe de mandarse el Bearer en la petición
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introduce el token JWT en el campo. Ejemplo: Bearer {token}"
    });

    //? Se añade esa definición de seguridad para todos los endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

// Middleware Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Catálogo v1");
        c.SwaggerEndpoint("/swagger/v2/swagger.json", "Catálogo v2");
        c.InjectStylesheet("/swagger-ui/custom.css"); //? Ruta relativa a wwwroot
    });

    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAll"); //* Se aplica la política de CORS para permitir los consumidores de la API
//app.UseHttpsRedirection(); //* Las peticiones se mandan a conexión segura
app.UseAuthentication(); //* Se autentica al usuario con el JWT
app.UseAuthorization(); //* Se autorizan los roles, permisos etc...
app.UseStaticFiles();
app.MapControllers();
app.Run();