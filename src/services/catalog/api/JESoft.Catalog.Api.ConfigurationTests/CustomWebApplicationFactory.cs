using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace JESoft.Catalog.Api.ConfigurationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        internal string ConfigurationSource = Environment.GetEnvironmentVariable("AzureAppConfigurationUri")
            ?? "AzureAppConfigurationUri Environment Variable missing, " +
            "consider running with .runsettings file " +
            "or specify the 'AzureAppConfigurationUri' environment variable using --environment|-e parameter ";
        internal string AppEnvironment = Environment.GetEnvironmentVariable("AppEnvironment")
           ?? "AppEnvironment Environment Variable missing, " +
           "consider running with .runsettings file " +
           "or specify the 'AzureAppConfigurationUri' environment variable using --environment|-e parameter ";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseEnvironment(AppEnvironment);
            var config = new ConfigurationBuilder().AddInMemoryCollection(
                    initialData: new Dictionary<string, string?>
                    {
                        ["ConfigurationConnectionString"] = ConfigurationSource
                    }).Build();
            builder.UseConfiguration(config);
        }
    }
}