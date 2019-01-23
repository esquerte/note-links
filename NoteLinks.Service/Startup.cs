using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NoteLinks.Data.Context;
using NoteLinks.Data.Repository.Implementations;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.Extensions;
using NSwag.AspNetCore;

namespace NoteLinks.Service
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MainContext>(options => options.UseSqlServer(connection));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddAutoMapper();            
            services.AddCors();
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            loggerFactory.AddFileLogger();

            app.UseCors(builder => builder.WithOrigins("http://localhost:4200", "http://localhost:64467")
                .AllowAnyHeader()
                .AllowAnyMethod());

            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;

                settings.PostProcess = document =>
                {
                    document.Info.Version = "v1";
                    document.Info.Title = "NoteLinks API";
                    document.Info.Description = "ASP.NET Core web API for simple online calendar";
                    document.Info.TermsOfService = "None";
                    document.Info.Contact = new NSwag.SwaggerContact
                    {
                        Name = "Alexander Pashkov",
                        Email = string.Empty,
                        Url = "https://github.com/esquerte/note-links"
                    };
                    document.Info.License = new NSwag.SwaggerLicense
                    {
                        Name = "Use under LICX",
                        Url = "https://example.com/license"
                    };
                };

            });

            app.UseHttpsRedirection();
            app.UseMvc();
            
        }
    }
}
