using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace AutoMediate
{
    /// <summary>
    /// Extension methods for IServiceCollection to register AutoMediate services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add AutoMediate services to the service collection
        /// </summary>
        public static IServiceCollection AddAutoMediate(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Register the mediator
            services.AddScoped<IMediator, Mediator>();

            // Register handlers from assemblies
            if (assemblies?.Length > 0)
            {
                services.AddRequestHandlers(assemblies);
            }

            return services;
        }

        /// <summary>
        /// Add AutoMediate services and scan assemblies containing the specified types
        /// </summary>
        public static IServiceCollection AddAutoMediate(this IServiceCollection services, params Type[] handlerAssemblyMarkerTypes)
        {
            var assemblies = handlerAssemblyMarkerTypes.Select(t => t.Assembly).Distinct().ToArray();
            return services.AddAutoMediate(assemblies);
        }

        /// <summary>
        /// Register request handlers from the specified assemblies
        /// </summary>
        internal static IServiceCollection AddRequestHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(IsHandlerInterface))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var interfaces = handlerType.GetInterfaces()
                    .Where(IsHandlerInterface)
                    .ToList();

                foreach (var interfaceType in interfaces)
                {
                    services.AddScoped(interfaceType, handlerType);
                }
            }

            return services;
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            var genericType = type.GetGenericTypeDefinition();
            return genericType == typeof(IRequestHandler<,>) || genericType == typeof(IRequestHandler<>);
        }
    }
}
