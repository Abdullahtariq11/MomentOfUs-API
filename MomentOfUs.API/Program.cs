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
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//Configure serilog
ServiceExtensions.ConfigureSerilog(builder.Host,builder.Configuration);
//Add database server
builder.Services.ConfigureSQLiteDatabase(builder.Configuration);
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJwtAuthentication(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddScoped<IRepositoryManager,RepositoryManager>();
builder.Services.AddScoped<IServiceManager,ServiceManager>();

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
