﻿using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NJsonSchema;
using NoteLinks.Data.Context;
using NoteLinks.Data.Repository.Implementations;
using NoteLinks.Data.Repository.Interfaces;
using NoteLinks.Service.ExceptionFilter;
using NoteLinks.Service.Extensions;
using NSwag.AspNetCore;
using System;
using System.Collections.Generic;

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

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var apiError = new ApiError(actionContext.ModelState);
                    apiError.Message = "Invalid model state.";
                    return new BadRequestObjectResult(apiError);
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
                app.UseDeveloperExceptionPage();
                loggerFactory.AddFileLogger(configure => configure.LogLevel = LogLevel.Warning);
            }
            else
            {
                app.UseHsts();
                loggerFactory.AddFileLogger(configure => configure.LogLevel = LogLevel.Error);
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
            app.UseMvc();
            
        }
    }
}
