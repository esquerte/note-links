using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NoteLinks.Data.Context;
using NoteLinks.Data.Repository;
using NoteLinks.Data.Repository.Implementations;
using NoteLinks.Data.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoteLinks.Data.Extentions
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();
            string connection = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MainDbContext>(options => options.UseSqlServer(connection));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
