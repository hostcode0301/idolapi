using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using idolapi.Helper.Authorization;
using idolapi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Diagnostics;
using Newtonsoft.Json.Serialization;
using System.Net;
using Newtonsoft.Json;
using idolapi.DB;
using idolapi.DB.DTOs;
using idolapi.Hubs;
using StackExchange.Redis.Extensions.Newtonsoft;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace idolapi
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
            // DB config
            string connectionString = Configuration.GetConnectionString("Postgre");
            services.AddDbContext<DataContext>(opt => opt.UseNpgsql(connectionString));
            services.AddScoped<DataContext, DataContext>();

            // Register the RedisCache service
            services.AddStackExchangeRedisExtensions<NewtonsoftSerializer>(Configuration.GetSection("Redis").Get<RedisConfiguration>());

            // Add validate JWT token middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero, // Disable default 5 mins of Microsoft
                    // ValidIssuer = Configuration["Jwt:Issuer"],
                    // ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            // Add IJwtGenerator to use in all project
            services.AddSingleton<IJWTGenerator>(new JWTGenerator(Configuration["Jwt:Key"]));

            // Map data from Model to DTO
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Custom Services register
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IIdolService, IdolService>();
            services.AddScoped<ICacheProvider, CacheProvider>();

            // Default service config
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                });

            // Setting JSON convert to camelCase in Object properties
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            services.AddSignalR();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "idolapi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "idolapi v1"));
            }

            // ReFormat exception message
            app.UseExceptionHandler(e => e.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerPathFeature>().Error;
                await context.Response.WriteAsJsonAsync(new ResponseDTO(500, exception.Message));
            }));

            app.UseHttpsRedirection();

            //Get front-end url from appsettings.json
            var frontEndUrl = Configuration["FrontEndUrl"];

            //CORS config for Front-end url
            app.UseCors(options => options.WithOrigins(frontEndUrl)
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials());

            // ReFormat error message
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(new ResponseDTO(401).ToString());
                }

                if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(new ResponseDTO(403).ToString());
                }
            });

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                // Map to SignalR hub
                endpoints.MapHub<NotifyHub>("/hubs/notification");
            });
        }
    }
}
