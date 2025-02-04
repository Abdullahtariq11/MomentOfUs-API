using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Configuration;
using MomentOfUs.API.Extensions;
using MomentOfUs.Application.Service;
using MomentOfUs.Application.Service.Contracts;
using MomentOfUs.Domain.Contracts;
using MomentOfUs.Infrastructure;
using MomentOfUs.Infrastructure.Repository;
using NLog;
using NLog.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
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


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();


//Add database server
builder.Services.ConfigureSQLiteDatabase(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalErrorHandlingMiddleware>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
