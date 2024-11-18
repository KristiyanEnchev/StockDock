namespace Application
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using FluentValidation;

    using MediatR;

    using Application.Common.Behaviours;
    using Application.Common.Mappings;

    using Shared.Mappings;

    public static class Startup
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddMapperConfig();
            services.AddValidators();
            services.AddMediator();
        }

        private static IServiceCollection AddMapperConfig(this IServiceCollection services)
        {
            var modelAssembly = Assembly.GetAssembly(typeof(Models.TokenSettings))
                ?? throw new InvalidOperationException(
                    $"Models assembly not found.");

            services.AddMappings(
                Assembly.GetExecutingAssembly(),
                modelAssembly,
                typeof(IMapFrom<>).Assembly
            );

            return services;
        }

        private static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private static void AddMediator(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
            });
        }
    }
}