using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.OpenApi.Models;
using MomentOfUs.API.Extensions;
using MomentOfUs.Application.Service;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Domain.Models;
using MomentOfUs.Infrastructure;
using MomentOfUs.Infrastructure.Repository;
using NLog;
using NLog.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
   // builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.

//Configure serilog
//ServiceExtensions.ConfigureSerilog(builder.Host,builder.Configuration);

//configure nlog
var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
logger.Info("Application Starting Up...");
// Ensure Serilog is the ONLY logging provider
//builder.Logging.ClearProviders(); // Removes default logging providers
//builder.Logging.AddSerilog();  // Adds Serilog as the only provider

// ✅ Configure NLog
builder.Logging.ClearProviders();  // Remove default logging providers
builder.Host.UseNLog();  // Add NLog as the main logging provider

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IJournalService,JournalService>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();




//Add database server
builder.Services.ConfigureSQLiteDatabase(builder.Configuration);
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
var jwtSecret = builder.Configuration["JwtSettings:Secret"];
var sqliteConn = builder.Configuration["ConnectionStrings:sqliteConnectionString"];

logger.Info($"JWT Secret From Env: {(string.IsNullOrWhiteSpace(jwtSecret) ? "NULL or EMPTY" : "Exists")}");
logger.Info($"SQLite Conn From Env: {(string.IsNullOrWhiteSpace(sqliteConn) ? "NULL or EMPTY" : sqliteConn)}");

builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token with 'Bearer' scheme",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },

        new string[] { }
    }});
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MomentOfUs Api",
        Version = "v1"
    });
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5063); // or whatever port you're using
});

var app = builder.Build();


//Apply CORS Before Middleware
app.UseCors("AllowAll");

// Configure the HTTP request pipeline.


    app.UseSwagger();
    app.UseSwaggerUI();


app.UseMiddleware<GlobalErrorHandlingMiddleware>();


//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
