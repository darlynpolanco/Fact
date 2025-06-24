using System.Reflection;
using Fact.Data;
using Fact.Features.Facturas.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictCORS", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Puerto EXACTO de Vite
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
    });
});

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(); 
}

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Configuraci�n de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configuraci�n de Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sistema de Facturaci�n API",
        Version = "v1",
        Description = "API para gesti�n de clientes, productos y facturas",
        Contact = new OpenApiContact
        {
            Name = "Soporte T�cnico",
            Email = "facturandocompany@gmail.com"
        }
    });
});

// Configuraci�n SMTP settings
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// Configuraci�n servicios de facturaci�n
builder.Services.AddScoped<IPdfGenerator, PdfGenerator>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Configuraci�n de Entity Framework Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuraci�n de FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Configuraci�n de MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

// Aplicar migraciones autom�ticamente al iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configuraci�n del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Facturaci�n API v1");
        c.DefaultModelsExpandDepth(-1); // Oculta los schemas por defecto
    });
}

// Endpoint de verificaci�n de salud
app.MapGet("/api/health", () =>
{
    Log.Information("Solicitud recibida en /api/health");
    return Results.Ok(new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
    });
});

app.MapControllers();

app.Use(async (context, next) =>
{
    Log.Information($"Recibida solicitud: {context.Request.Method} {context.Request.Path}");
    await next();
    Log.Information($"Respuesta enviada: {context.Response.StatusCode}");
});

app.UseCors("StrictCORS");

app.UseAuthorization();
app.MapControllers();

app.Run();
Log.CloseAndFlush();