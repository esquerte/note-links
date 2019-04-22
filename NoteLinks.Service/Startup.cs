using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema;
using NoteLinks.Data.Context;
using NoteLinks.Data.Entities;
using NoteLinks.Data.Repository.Implementations;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.Authentication;
using NoteLinks.Service.ExceptionHandling;
using NoteLinks.Service.Extensions;
using NoteLinks.Service.Logging.Service;
using NSwag.AspNetCore;
using System;
using Hosting = Microsoft.Extensions.Hosting;

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
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            services.AddIdentityCore<User>(options => {
                    options.User.RequireUniqueEmail = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 8;
                    options.Password.RequiredUniqueChars = 1;
                })
                .AddEntityFrameworkStores<MainContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthOptions.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = AuthOptions.AUDIENCE,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),

                        ClockSkew = TimeSpan.FromSeconds(5)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {                          
                            // Skip the default logic.
                            context.HandleResponse();

                            string message = String.IsNullOrEmpty(context.Error)
                                ? "Unauthorized access"
                                : $"{context.Error}";

                            message += String.IsNullOrEmpty(context.ErrorDescription)
                                ? "."
                                : $": {context.ErrorDescription}.";

                            throw new ApiException(message, StatusCodes.Status401Unauthorized);
                        }
                    };
                });

            services.AddMvc()
                .AddJsonOptions(options => 
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                )
                .AddFluentValidation(options =>
                {
                    options.RegisterValidatorsFromAssemblyContaining<Startup>();
                    options.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    options.LocalizationEnabled = false;
                });

            services.AddSingleton<Hosting.IHostedService, LoggingService>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    throw new ApiException("Invalid model state.", StatusCodes.Status400BadRequest);
                };
            });

            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<MainContext>(options => options.UseSqlServer(connection));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            
            services.AddAutoMapper();            
            services.AddCors();
            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IAntiforgery antiforgery)
        {
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            app.Use(async (context, next) =>
            {
                string path = context.Request.Path.Value;
                if (path != null)
                {
                    // XSRF-TOKEN used by angular in the $http if provided
                    var tokens = antiforgery.GetAndStoreTokens(context);
                    context.Response.Cookies.Append("XSRF-TOKEN",
                      tokens.RequestToken, new CookieOptions
                      {
                          HttpOnly = false,
                          Expires = DateTimeOffset.Now.AddMinutes(10),
                          Path = "/"
                      });
                }
                await next();
            });

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                loggerFactory.AddFileLogger(configure => configure.LogLevel = LogLevel.Warning);
            }
            else
            {
                app.UseHsts();
                loggerFactory.AddFileLogger(configure => configure.LogLevel = LogLevel.Warning);
            }

            app.UseCors(builder => builder.WithOrigins("http://localhost:4200", "http://localhost:64467")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials());

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
            app.UseAuthentication();            
            app.UseMvc();
            
        }
    }
}
