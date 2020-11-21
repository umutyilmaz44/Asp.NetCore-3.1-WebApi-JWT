using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using base_app_repository.Entities;
using base_app_service;
using base_app_webapi.Helper;
using base_app_webapi.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace base_app_webapi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<BaseDbContext>(options => options.UseNpgsql(connectionString));

            // configure strongly typed settings objects
            var jwtSettingsSection = Configuration.GetSection("JWTSettings");
            services.Configure<JWTSettings>(jwtSettingsSection);

            //to validate the token which has been sent by clients
            var appSettings = jwtSettingsSection.Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    TokenDecryptionKey = new SymmetricSecurityKey(key)
                };
                x.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            IdentityModelEventSource.ShowPII = true;

            var swaggerRestrictionsSection = Configuration.GetSection("SwaggerRestrictions");
            var swaggerRestrictions = swaggerRestrictionsSection.Get<SwaggerRestrictions>();
            services.AddSwaggerGen(gen =>
            {
                gen.SwaggerDoc("v1.0", new Microsoft.OpenApi.Models.OpenApiInfo { Title = appSettings.Audience, Version = "v1.0" });
                gen.DocumentFilter<SwaggerFilterOutControllers>(swaggerRestrictions);
                gen.CustomSchemaIds(x => GetCustomSchemaId(x));
                gen.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.ApiKey,  
                    Scheme = "Bearer",  
                    BearerFormat = "JWT",  
                    In = ParameterLocation.Header,  
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",  
                });  
                gen.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  
                          new OpenApiSecurityScheme  
                            {  
                                Reference = new OpenApiReference  
                                {  
                                    Type = ReferenceType.SecurityScheme,  
                                    Id = "Bearer"  
                                }  
                            },  
                            new string[] {}  
  
                    }  
                }); 
            });

            services.TryAddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();            
            services.AddScoped<IServiceManager, ServiceManager>();
            services.AddAutoMapper(typeof(Startup), typeof(AutoMapperProfile));
            services.AddSingleton<IMailer, Mailer>();

            services.AddCors(); // Make sure you call this previous to AddMvc

            services.AddControllers()
                    .AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            string audience = Configuration["JWTSettings:Audience"];

            app.UseSwagger();
            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint("/swagger/v1.0/swagger.json", audience + " Endpoint");
                ui.RoutePrefix = string.Empty;
            });

            app.UseMiddleware<IpSafeListMiddleware>(Configuration["ClientIpSafeList"]);

            app.UseExceptionHandler(config =>
            {
                config.Run(async context =>
                {
                    await Task.Run(() =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            var logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
                            var ex = error.Error;
                            logger.Error(ex, "Unhandled exception!");
                        }
                    });                    
                });
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    
        public string GetCustomSchemaId(Type type)
        {
            try
            {
                string response ="";
                if(!(type.IsGenericType && type.GenericTypeArguments.Length > 0))
                    response = type.Name;
                else 
                {
                    string name = type.Name.Substring(0, type.Name.IndexOf("`")) + "<{type_name}>";
                    string type_name = GetCustomSchemaId(type.GenericTypeArguments[0]);
                    response = name.Replace("{type_name}",type_name);
                    //type.GenericTypeArguments[0].Name
                }
                Console.WriteLine(type.Name + " => " + response);
                return response;
            }
            catch(Exception ex)
            {
                Console.WriteLine(type.Name);
                Console.WriteLine(ex);
                return type.Name;
            }
        }
    }
}
