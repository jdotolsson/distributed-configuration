using JESoft.AspNet.Configuration.Attributes;
using JESoft.AspNet.Configuration.Exceptions;
using System.Reflection;

namespace JESoft.AspNet.Configuration
{
    internal static class OptionsValidator
    {
        internal static void ValidateUniqueOptions(Type targetType)
        {
            ValidateUniqueOptions(targetType, new HashSet<(Type, string)>());
        }

        private static void ValidateUniqueOptions(Type targetType, HashSet<(Type, string)> processedTypes)
        {
            var attributeType = typeof(AddOptionsValidationAttribute);
            foreach (var property in targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attribute = (AddOptionsValidationAttribute?)property.GetCustomAttribute(attributeType);
                if (attribute != null)
                {
                    if (!processedTypes.Add((property.PropertyType, attribute.Name)))
                    {
                        throw new DuplicateOptionInstanceException(property.PropertyType);
                    }
                }
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    ValidateUniqueOptions(property.PropertyType, processedTypes);
                }
            }
        }
    }
}