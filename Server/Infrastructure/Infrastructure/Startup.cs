namespace Infrastructure
{
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using MediatR;

    using StackExchange.Redis;

    using Persistence.Context;

    using Models;

    using Infrastructure.Services.Token;
    using Infrastructure.Services.Cache;
    using Infrastructure.Services.Identity;

    using Application.Interfaces.Cache;
    using Application.Interfaces.Identity;

    using Domain.Entities.Identity;

    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddServices()
                .AddRedis(configuration)
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
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/stock"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<TokenSettings>(configuration.GetSection(nameof(TokenSettings)));

            return services;
        }

        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
            services.Configure<RedisSettings>(configuration.GetSection(nameof(RedisSettings)));

            if (string.IsNullOrEmpty(redisSettings.ConnectionString))
            {
                throw new InvalidOperationException("Redis connection string is missing in configuration.");
            }

            if (redisSettings.ConnectionString.StartsWith("default:"))
            {
                var parts = redisSettings.ConnectionString.Replace("default:", "").Split('@');
                if (parts.Length != 2) throw new InvalidOperationException("Invalid Upstash Redis connection string format.");

                redisSettings.ConnectionString = $"{parts[1]},password={parts[0]},ssl=True,abortConnect=False";
            }

            if (!Uri.TryCreate($"rediss://{redisSettings.ConnectionString.Split(',')[0]}", UriKind.Absolute, out var redisUri))
            {
                throw new InvalidOperationException("Invalid Redis connection string format.");
            }

            var redisOptions = ConfigurationOptions.Parse(redisSettings.ConnectionString, true);

            var host = redisUri.Host;
            var port = redisUri.Port > 0 ? redisUri.Port : 6379;
            var password = redisOptions.Password ?? redisUri.UserInfo;
            var sslEnabled = redisOptions.Ssl;

            var redisConfigString = $"{host}:{port},ssl={sslEnabled.ToString().ToLower()},abortConnect=False,password={password}";

            var options = new ConfigurationOptions
            {
                EndPoints = { $"{host}:{port}" },
                Ssl = sslEnabled,
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry(5000),
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                DefaultDatabase = 0,
                ResolveDns = true,
                KeepAlive = 60,
                Password = password
            };

            var multiplexer = ConnectionMultiplexer.Connect(options);
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConfigString;
                options.InstanceName = redisSettings.InstanceName;
            });

            services
                .AddTransient<ICacheService, RedisCacheService>();

            return services;
        }
    }
}