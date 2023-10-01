namespace JESoft.AspNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AddOptionsValidationAttribute : Attribute
    {

        /// <summary>
        /// Property i  s used when mulitple options exist with the same type.
        /// This will be a named option
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}