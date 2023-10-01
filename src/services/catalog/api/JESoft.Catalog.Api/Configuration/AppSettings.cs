using JESoft.AspNet.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace JESoft.Catalog.Api.Configuration
{
    public class AppSettings
    {
        [AddOptionsValidation]
        public CatalogSettings Catalog { get; set; } = new();
    }

    public class CatalogSettings
    {
        [AddOptionsValidation]
        public DataStoreSettings Datastore { get; set; } = new();
        [AddOptionsValidation]
        public ServiceEndpointSettings Service { get; set; } = new();
    }

    public class DataStoreSettings
    {
        [Required, Url]
        public string CosmosDb { get; set; } = string.Empty;
    }

    public class ServiceEndpointSettings
    {
        [AddOptionsValidation]
        public EndpointSettings Endpoint { get; set; } = new();
    }

    public class EndpointSettings
    {
        public string Endpoint { get; set; } = string.Empty;
        [Required]
        public string RequiredProperty { get; set; } = string.Empty;
        [StringLength(10)]
        public string StringLengthProperty { get; set; } = "A String longer than 10 characters";
        [RegularExpression("[a-z]")]
        public string RegularExpressionProperty { get; set; } = "ONLY LOWERCASE";
        [Range(0, 10)]
        public string RangeProperty { get; set; } = "11";
    }
}
