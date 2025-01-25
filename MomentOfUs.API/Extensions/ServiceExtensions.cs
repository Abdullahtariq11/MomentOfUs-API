using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MomentOfUs.Infrastructure;

namespace MomentOfUs.API.Extensions
{
    /// <summary>
    /// Register all services here
    /// </summary>
    public static class ServiceExtensions
    {
        public static void ConfigureSQLiteDatabase(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(opts=>opts.UseSqlite(configuration.GetConnectionString("sqliteConnectionString")));
        }
    }
}