using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MomentOfUs.Infrastructure;
using Serilog;

namespace MomentOfUs.API.Extensions
{
    /// <summary>
    /// Register all services here
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Configuring sqlite database
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureSQLiteDatabase(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(opts=>opts.UseSqlite(configuration.GetConnectionString("sqliteConnectionString")));
        }
        /// <summary>
        /// Configuring Serilog Logger
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="configuration"></param>
        public static void ConfigureSerilog(IHostBuilder hostBuilder, IConfiguration configuration)
        {
            Log.Logger= new LoggerConfiguration()
            .ReadFrom.Configuration(configuration.GetSection("Serilog")) //load config from appsetting.json
            .Enrich.FromLogContext()
            .CreateLogger();

            hostBuilder.UseSerilog(Log.Logger); //attaches serilog to host
        }
    }
    
}