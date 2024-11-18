namespace Web.Extensions.Healtchecks
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;

    using Models.HealthCheck;

    internal static class HealtExtension
    {
        internal static IServiceCollection AddHealth(this IServiceCollection services, IConfiguration configuration)
        {
            var healthSettings = configuration.GetSection(nameof(Health)).Get<Health>();

            services.AddSingleton<CustomHealthCheckResponseWriter>();

            var databaseHealthChecks = healthSettings?.DatabaseHealthChecks;

            var healthChecks = services.AddHealthChecks();

            if (databaseHealthChecks != null && (bool)databaseHealthChecks)
            {
                healthChecks.AddNpgSql(configuration.GetConnectionString("DefaultConnection")!);
            }

            healthChecks.AddCheck<ControllerHealthCheck>("controller_health_check");

            healthChecks.AddCheck("disk_space_health_check",
            new DiskSpaceHealthCheck());

            healthChecks.AddCheck("memory_health_check",
            new MemoryHealthCheck(maxAllowedMemory: 1024L * 1024L * 1024L));

            return services;
        }

        public class ControllerHealthCheck : IHealthCheck
        {
            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                bool isControllerHealthy = true;

                if (isControllerHealthy)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Controller is healthy"));
                }

                return Task.FromResult(HealthCheckResult.Unhealthy("Controller is unhealthy"));
            }
        }

        public class CacheHealthCheck : IHealthCheck
        {
            private readonly IMemoryCache _cache;
            private readonly string _testCacheKey = "health_check_cache_key";
            private readonly string _testCacheValue = "test_value";

            public CacheHealthCheck(IMemoryCache cache)
            {
                _cache = cache;
            }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                try
                {
                    _cache.Set(_testCacheKey, _testCacheValue);

                    if (_cache.TryGetValue(_testCacheKey, out string cachedValue))
                    {
                        if (cachedValue == _testCacheValue)
                        {
                            return Task.FromResult(HealthCheckResult.Healthy("Cache is healthy"));
                        }
                    }

                    return Task.FromResult(HealthCheckResult.Unhealthy("Cache is not working as expected"));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Cache health check failed", ex));
                }
            }
        }

        public class DiskSpaceHealthCheck : IHealthCheck
        {
            public DiskSpaceHealthCheck() { }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                var metrics = new Dictionary<string, object>();
                try
                {
                    var drives = DriveInfo.GetDrives()
                        .Where(d => d.IsReady && (d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Network));

                    foreach (var drive in drives)
                    {
                        var freeSpace = drive.AvailableFreeSpace;
                        var totalSpace = drive.TotalSize;
                        var freeSpacePercent = (double)freeSpace / totalSpace * 100;

                        metrics.Add($"drive_{drive.Name}_free_gb", freeSpace / 1024.0 / 1024.0 / 1024.0);
                        metrics.Add($"drive_{drive.Name}_free_percent", freeSpacePercent);

                        if (freeSpace < 10L * 1024L * 1024L * 1024L)
                        {
                            return Task.FromResult(new HealthCheckResult(
                                HealthStatus.Degraded,
                                $"Low disk space on drive {drive.Name}",
                                data: metrics));
                        }
                    }

                    return Task.FromResult(new HealthCheckResult(
                        HealthStatus.Healthy,
                        "Sufficient disk space available",
                        data: metrics));
                }
                catch (Exception ex)
                {
                    return Task.FromResult(new HealthCheckResult(
                        HealthStatus.Unhealthy,
                        "Disk space check failed",
                        ex,
                        metrics));
                }
            }
        }

        public class MemoryHealthCheck : IHealthCheck
        {
            private readonly long _maxAllowedMemory;

            public MemoryHealthCheck(long maxAllowedMemory)
            {
                _maxAllowedMemory = maxAllowedMemory;
            }

            public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
            {
                var memoryUsed = GC.GetTotalMemory(false);

                if (memoryUsed <= _maxAllowedMemory)
                {
                    return Task.FromResult(HealthCheckResult.Healthy("Memory usage is within limits"));
                }

                return Task.FromResult(HealthCheckResult.Unhealthy($"Memory usage is too high. Current usage: {memoryUsed / 1024 / 1024} MB"));
            }
        }

        internal static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
            endpoints.MapHealthChecks("/api/health", new HealthCheckOptions
            {
                ResponseWriter = (httpContext, result) => CustomHealthCheckResponseWriter.WriteResponse(httpContext, result),
            });
    }
}