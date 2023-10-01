using JESoft.AspNet.Configuration.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace JESoft.AspNet.Configuration.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOptionsValidationRecursivly<T>(this IServiceCollection services) where T : class
        {
            OptionsValidator.ValidateUniqueOptions(typeof(T));
            services.AddOptions<T>()
              .BindConfiguration(string.Empty)
              .ValidateDataAnnotations()
              .ValidateOnStart();

            var nestedOptions = DiscoverNestedOptions(typeof(T));
            services.RegisterNestedOptions(nestedOptions);
            return services;
        }

        private static List<(PropertyInfo PropertyInfo, string ConfigSection)> DiscoverNestedOptions(Type type, string configSection = "")
        {
            var nestedOptions = new List<(PropertyInfo PropertyInfo, string ConfigSection)>();

            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var nestedConfigSection = string.IsNullOrEmpty(configSection) ? propertyInfo.Name : $"{configSection}:{propertyInfo.Name}";
                var propertyType = propertyInfo.PropertyType;

                if (propertyType.IsClass && propertyType != typeof(string))
                {
                    var attribute = propertyInfo.GetCustomAttribute<AddOptionsValidationAttribute>();

                    if (attribute != null)
                    {
                        nestedOptions.Add((propertyInfo, nestedConfigSection));
                    }

                    nestedOptions.AddRange(DiscoverNestedOptions(propertyType, nestedConfigSection));
                }
            }

            return nestedOptions;
        }

        private static void RegisterNestedOptions(this IServiceCollection services, List<(PropertyInfo PropertyInfo, string ConfigSection)> nestedOptions)
        {
            foreach (var (propertyInfo, nestedConfigSection) in nestedOptions)
            {
                var attribute = propertyInfo.GetCustomAttribute<AddOptionsValidationAttribute>();
                if (attribute != null)
                {
                    var optionsName = attribute.Name == Options.DefaultName ? Options.DefaultName : attribute.Name;
                    _ = typeof(ServiceCollectionExtensions)
                        .GetMethod(nameof(AddCustomOptions), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)?
                        .MakeGenericMethod(propertyInfo.PropertyType)
                        .Invoke(null, new object[] { services, optionsName, nestedConfigSection })
                        ?? throw new MissingMethodException(nameof(ServiceCollectionExtensions), nameof(AddCustomOptions));
                }
            }
        }

        private static OptionsBuilder<T> AddCustomOptions<T>(this IServiceCollection services, string optionsName, string configurationSection) where T : class
        {
            return services.AddOptions<T>(optionsName)
              .BindConfiguration(configurationSection)
              .ValidateDataAnnotations()
              .ValidateOnStart();
        }
    }
}