namespace Web
{
    using System.Reflection;
    using System.Security.Authentication;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Server.Kestrel.Https;

    using StackExchange.Redis;

    using Application;
    using Application.Interfaces.Cache;
    using Application.Interfaces.Stock;
    using Application.Interfaces.Identity;
    using Application.Interfaces.Alerts;
    using Application.Interfaces.Watchlist;

    using Web.Services;
    using Web.Extensions.Swagger;
    using Web.Extensions.Middleware;
    using Web.Extensions.Healtchecks;

    using Infrastructure;
    using Infrastructure.Services.Cache;
    using Infrastructure.Services.Stock;
    using Infrastructure.Services.Alerts;

    using Persistence;
    using Persistence.Context;

    using Models;

    public static class Startup
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly()).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            });

            services.AddCustomSignalR(config);
            services.AddApplication();
            services.AddInfrastructure(config);
            services.AddPersistence(config);

            services.AddSwaggerDocumentation();
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddCors();

            services.AddHealth(config);
            services.AddScoped<IUser, CurrentUser>();

            services.AddHttpClient<IExternalStockApi, AlphaVantageClient>(client =>
            {
                client.BaseAddress = new Uri("https://www.alphavantage.co/");
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Add("User-Agent", "StockDock");
            });

            services.AddSingleton<ICacheService, RedisCacheService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IWatchlistService, WatchlistService>();
            services.AddScoped<IStockAlertService, StockAlertService>();
            services.AddHostedService<StockUpdateService>();

            return services;
        }

        public static IServiceCollection AddCustomSignalR(this IServiceCollection services, IConfiguration config)
        {
            var redisSettings = config.GetSection(nameof(RedisSettings)).Get<RedisSettings>();
            if (redisSettings == null)
            {
                throw new InvalidOperationException("Redis settings are not configured properly in appsettings.json");
            }

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
                options.MaximumReceiveMessageSize = 32 * 1024;
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
            })
            .AddStackExchangeRedis(redisSettings.ConnectionString, options =>
            {
                options.Configuration.ChannelPrefix = "StockHub";
            });

            return services;
        }

        public static async Task InitializeDatabase(this IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

            await initialiser.InitialiseAsync();

            await initialiser.SeedAsync();
        }

        public static IServiceCollection AddConfigurations(this IServiceCollection services, IWebHostBuilder hostBulder, IWebHostEnvironment env)
        {
            hostBulder.ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                config.AddEnvironmentVariables();
                config.Build();
            });

            AddKestrelConfig(hostBulder);

            return services;
        }

        private static IWebHostBuilder AddKestrelConfig(IWebHostBuilder builder)
        {
            builder.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.ListenAnyIP(8080);

                serverOptions.ConfigureHttpsDefaults(options =>
                {
                    options.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                    options.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                });
            });

            return builder;
        }

        public static IApplicationBuilder UseWeb(this IApplicationBuilder builder)
        {
            builder.UseSwaggerDocumentation()
                    .UseStaticFiles()
                    .UseHttpsRedirection()
                    .UseErrorHandler()
                    .UseRouting()
                    .UseCors("CleanArchitecture")
                    .UseAuthentication()
                    .UseAuthorization()
                    .UseEndpoints(endpoints => endpoints.MapEndpoints());

            return builder;
        }

        public static IServiceCollection AddCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CleanArchitecture",
                    builder => builder
                       .AllowAnyHeader()
                       .AllowAnyMethod()
                       .AllowCredentials()
                       .WithOrigins("http://localhost:3000"));
            });

            return services;
        }

        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            var redisSettings = configuration.GetSection(nameof(RedisSettings)).Get<RedisSettings>();

            if (redisSettings == null)
            {
                throw new InvalidOperationException("Redis settings are not configured properly in appsettings.json");
            }

            services.Configure<RedisSettings>(configuration.GetSection(nameof(RedisSettings)));

            var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { redisSettings.ConnectionString },
                AbortOnConnectFail = false,
                ReconnectRetryPolicy = new ExponentialRetry(5000),
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                DefaultDatabase = 0,
                ResolveDns = true,
                KeepAlive = 60,
            });

            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
                options.InstanceName = redisSettings.InstanceName;
            });

            services.AddSingleton<IConnectionMultiplexer>(sp =>
                ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

            return services;
        }

        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        {
            builder.MapControllers();
            builder.MapHealthCheck();
            builder.MapHub<StockHub>("/hubs/stock");

            return builder;
        }
    }
}