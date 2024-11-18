﻿namespace Infrastructure
{
    using System.Text;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using MediatR;

    using Persistence.Context;

    using Domain.Entities;

    using Models;

    using Infrastructure.Services.Identity;
    using Infrastructure.Services.Token;

    using Application.Interfaces;
    using System.Text.Json.Serialization;
    using System.Text.Json;

    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddServices()
                .AddConfigurations(configuration)
                .AddIdentity(configuration)
                .AddCustomAuthentiation(configuration);

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            services.AddSingleton(jsonOptions);

            services
                .AddTransient<IMediator, Mediator>()
                .AddTransient<IUserService, UserService>();

            return services;
        }

        private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddTransient<IIdentityService, IdentityService>()
                .AddTransient<IJwtService, JwtService>()
                .AddIdentity<User, UserRole>(options =>
                {
                    options.Password.RequiredLength = 6;
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider("CleanArchitecture", typeof(DataProtectorTokenProvider<User>));

            return services;
        }

        public static IServiceCollection AddCustomAuthentiation(this IServiceCollection services, IConfiguration configuration)
        {
            var key = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Key))!;
            var audience = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Audience))!;
            var issuer = configuration.GetSection(nameof(TokenSettings)).GetValue<string>(nameof(TokenSettings.Issuer))!;

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
                };
            });

            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenSettings>(configuration.GetSection(nameof(TokenSettings)));

            return services;
        }
    }
}