using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

using KIOT.Server.Api.Helpers;
using KIOT.Server.Api.Services;
using KIOT.Server.Business;
using KIOT.Server.Business.Handler;
using KIOT.Server.Business.Request.Application;
using KIOT.Server.Core.Services;
using KIOT.Server.Data;
using KIOT.Server.Data.Persistence.Identity;
using KIOT.Server.Data.External;
using KIOT.Server.Dto.Application.Authentication;

namespace KIOT.Server.Api
{
    public static class ConfigureServicesExtensions
    {
        public static void AddHangFireJobs(this IServiceProvider provider)
        {
            using (var scope = provider.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<ICustomerService>();
                BackgroundJob.Enqueue(() => service.RunTasks());
                RecurringJob.AddOrUpdate(() => service.RunTasks(), Cron.MinuteInterval(5));
            }
        }

        public static IServiceCollection AddInternalServices(this IServiceCollection services)
        {
            return services.RegisterDataServices()
                .RegisterApiServices()
                .RegisterBusinessServices()
                .AddSingleton<IBackgroundTaskService, BackgroundTaskService>();
        }

        public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
        {
            return services.AddAutoMapper(Assembly.GetAssembly(typeof(MappingProfile)),
                Assembly.GetAssembly(typeof(HandlerMappingProfile)));
        }

        public static IServiceCollection AddMediatRServices(this IServiceCollection services)
        {
            return services.AddMediatR(Assembly.GetAssembly(typeof(BusinessExtensions)));
        }

        public static IMvcBuilder AddFluentValidation(this IMvcBuilder builder)
        {
            return builder.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<RegisterCaretakerRequestValidator>();
                fv.RegisterValidatorsFromAssemblyContaining<RegisterCaretakerDto>();
            });
        }

        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            return services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "KIOT.Server.Api", Version = "v1" });
                s.AddFluentValidationRules();
                s.OperationFilter<AuthorizationOperationFilter>();

                s.DescribeAllEnumsAsStrings();
                s.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new Microsoft.OpenApi.Models.OpenApiSecurityScheme()
                {
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "JWT Authorization using Bearer scheme"
                });


                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            
                //s.IncludeXmlComments(xmlPath);
            });
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<ITokenConfiguration>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var jwtParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromTicks(0),
                IssuerSigningKey = new SymmetricSecurityKey(config.SigningKey),
                NameClaimType = "sub",
                RoleClaimType = "role",
                ValidAudience = config.Audience,
                ValidateAudience = true,
                ValidIssuer = config.Issuer,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.ClaimsIssuer = config.Issuer;
                    options.SaveToken = true;
                    options.TokenValidationParameters = jwtParameters;
                });
            return services;
        }

        public static IServiceCollection AddAuthorization(this IServiceCollection services)
        {
            return services.AddAuthorization(opt => { });
        }
    }
}
