using API_AlfaTorres.Repositories;
using API_AlfaTorres.Data;
using API_AlfaTorres.Filters;
using API_AlfaTorres.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using API_AlfaTorres;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
   .ReadFrom.Configuration(builder.Configuration) // This requires Serilog.Extensions.Logging package  
   .Enrich.FromLogContext()
   .CreateLogger();

builder.Host.UseSerilog();
builder.Host.UseWindowsService();

// Agrega los servicios al contenedor.  
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API ALFATORRES", Version = "v1" });

    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "Introduce el token fijo en el campo. Ejemplo: Bearer {tu_token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKey"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
   {
       {
           new OpenApiSecurityScheme
           {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "ApiKey"
               }
           },
           new string[] {}
       }
   });
});

builder.Services.AddDbContext<SageDbContext>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ArticulosRepository>();
builder.Services.AddScoped<EscandalloRespository>();

var app = builder.Build();

// Configura el pipeline HTTP.  
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API ALFATORRES v1");
    c.RoutePrefix = string.Empty;
});

OEM.oem = new LcOEM(builder.Configuration);

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
