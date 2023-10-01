using JESoft.AspNet.Configuration.Attributes;

namespace JESoft.AspNet.Configuration.Exceptions
{
    public class DuplicateOptionInstanceException : Exception
    {
        public DuplicateOptionInstanceException(Type targetType)
            : base($"Type '{targetType.FullName}' is defined more than once with the attribute '{typeof(AddOptionsValidationAttribute).FullName}'." +
            $"Consider changing type or use (different) named options using the {nameof(AddOptionsValidationAttribute.Name)} property")
        {

        }
    }
}