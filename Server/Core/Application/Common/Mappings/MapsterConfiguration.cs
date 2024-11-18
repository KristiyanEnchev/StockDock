namespace Application.Common.Mappings
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Mapster;
    using MapsterMapper;

    public static class MapsterConfig
    {
        public static IServiceCollection AddMappings(this IServiceCollection services, params Assembly[] assemblies)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(assemblies);

            var config = TypeAdapterConfig.GlobalSettings;

            config.Default
                .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                .PreserveReference(true)
                .ShallowCopyForSameType(true);

            foreach (var assembly in assemblies)
            {
                ApplyMappingsFromAssembly(config, assembly);
            }

            try
            {
                config.Compile();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error configuring Mapster mappings: {ex.Message}", ex);
            }

            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }

        private static void ApplyMappingsFromAssembly(TypeAdapterConfig config, Assembly assembly)
        {
            var types = assembly.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface &&
                    t.GetInterfaces()
                        .Any(i => i.IsGenericType &&
                             i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
                .ToList();

            foreach (var type in types)
            {
                var instance = Activator.CreateInstance(type);
                var methodInfo = type.GetMethod("Mapping")
                               ?? type.GetInterface("IMapFrom`1")?.GetMethod("Mapping");

                methodInfo?.Invoke(instance, new object[] { config });

                var customizeMapping = type.GetMethod("CustomizeMapping");
                customizeMapping?.Invoke(instance, new object[] { config });
            }
        }
    }
}