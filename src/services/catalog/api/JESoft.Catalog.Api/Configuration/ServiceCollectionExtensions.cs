using Azure.Identity;

namespace JESoft.Catalog.Api.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IConfiguration AddCustomConfiguration(this IConfigurationBuilder configurationBuilder)
        {
            var intermediateConfig = configurationBuilder.Build();
            var configConnectionString = intermediateConfig.GetValue<string>("ConfigurationConnectionString");
            ArgumentException.ThrowIfNullOrEmpty(configConnectionString, nameof(configConnectionString));
            var environment = intermediateConfig.GetValue<string>("Environment");
            ArgumentException.ThrowIfNullOrEmpty(environment, nameof(environment));

            var credentials = new DefaultAzureCredential();
            configurationBuilder.AddAzureAppConfiguration(config =>
            {
                config
                .Connect(new Uri(configConnectionString), credentials)
                .ConfigureKeyVault(options =>
                {
                    options.SetCredential(credentials);
                })
                .Select("*", $"{environment}-{Constants.ApplicationName}");
            });

            return configurationBuilder.Build();
        }
    }
}
