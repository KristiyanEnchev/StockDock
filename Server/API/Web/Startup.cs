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

    using Application;
    using Application.Interfaces.Stock;
    using Application.Interfaces.Identity;
    using Application.Interfaces.Alerts;
    using Application.Interfaces.Watchlist;

    using Web.Services;
    using Web.Extensions.Json;
    using Web.Extensions.Swagger;
    using Web.Extensions.Middleware;
    using Web.Extensions.Healtchecks;

    using Infrastructure;
    using Infrastructure.Services.Stock;
    using Infrastructure.Services.Alerts;
    using Infrastructure.Services.Demo;
    using Infrastructure.Services.BackgroundServices;

    using Persistence;
    using Persistence.Context;

    using Models;

    public static class Startup
    {
        public static IServiceCollection AddWeb(this IServiceCollection services, IConfiguration config)
        {
            services.AddHttpContextAccessor();
            services.AddControllers()
                .AddApplicationPart(Assembly.GetExecutingAssembly())
                .ConfigureJsonOptions();

            services.AddCustomSignalR(config);
            services.AddApplication();
            services.AddInfrastructure(config);
            services.AddPersistence(config);

            services.AddSwaggerDocumentation();
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddCorsPolicy(config);

            services.AddHealth(config);
            services.AddScoped<IUser, CurrentUser>();

            services.AddStockServices(config);

            return services;
        }

        private static IServiceCollection AddStockServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StockApiSettings>(configuration.GetSection("StockApi"));
            var stockApiSettings = configuration.GetSection("StockApi").Get<StockApiSettings>();

            if (stockApiSettings?.UseDemo == true)
            {
                services.AddSingleton<DemoStockDataProvider>();
                services.AddScoped<IExternalStockApi>(sp => sp.GetRequiredService<DemoStockDataProvider>());
                services.AddHostedService<DemoStockUpdateService>();
                services.AddScoped<IStockService, DemoStockService>();
                services.AddScoped<StockDatabaseInitializer>();
            }
            else
            {
                services.AddHttpClient<IExternalStockApi, AlphaVantageClient>(client =>
                {
                    client.BaseAddress = new Uri("https://www.alphavantage.co/");
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("User-Agent", "StockDock");
                });

                services.AddHostedService<StockUpdateService>();
                services.AddScoped<IStockService, StockService>();
            }

            services.AddScoped<IWatchlistService, WatchlistService>();
            services.AddScoped<IStockAlertService, StockAlertService>();

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

            var dbInitializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();
            await dbInitializer.InitialiseAsync();
            await dbInitializer.SeedAsync();

            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var stockApiSettings = config.GetSection("StockApi").Get<StockApiSettings>();

            if (stockApiSettings?.UseDemo == true)
            {
                var demoInitializer = scope.ServiceProvider.GetRequiredService<StockDatabaseInitializer>();
                await demoInitializer.InitializeAsync();
            }
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

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
        {
            var originsString = config.GetValue<string>("Cors:Origins");

            if (string.IsNullOrWhiteSpace(originsString))
            {
                return services;
            }

            var origins = originsString.Split(';', StringSplitOptions.RemoveEmptyEntries);

            services.AddCors(opt =>
                opt.AddPolicy("CleanArchitecture", policy =>
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(origins)));

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