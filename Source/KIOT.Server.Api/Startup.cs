using System;
using System.Security.Claims;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace KIOT.Server.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("Api Started");
            services.AddInternalServices()
                .AddAutoMapperServices()
                .AddMediatRServices()
                .AddSwaggerServices()
                .AddAuthentication()
                .AddAuthorization();

            services.AddScoped(p => 
                p.GetService<IHttpContextAccessor>().HttpContext.User.Identity as ClaimsIdentity);

            services.AddHangfire((c) => { c.UseMemoryStorage(); });

            services.AddCors(options =>
            {
                options.AddPolicy("Cors",
                builder => builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                .Build());
            });


            services.AddControllers().AddNewtonsoftJson(options =>
                  {
                      options.SerializerSettings.ContractResolver =
                          new CamelCasePropertyNamesContractResolver();
                  })
                .AddFluentValidation()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment() || env.IsDevelopment())
            {
                app.UseSwaggerUI(s =>
                {
                    s.SwaggerEndpoint("/swagger/v1/swagger.json", "KIOT.Server.Api V1");
                });

                app.UseDeveloperExceptionPage();
                app.UseHsts();
            }
            else
            {
            }

            var services = app.ApplicationServices;

            app.UseHangfireServer();

            if (env.IsProduction())
            {
                services.AddHangFireJobs();
            }

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseCors("Cors");
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
