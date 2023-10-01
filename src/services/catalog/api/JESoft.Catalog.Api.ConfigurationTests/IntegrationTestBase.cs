namespace JESoft.Catalog.Api.ConfigurationTests
{
    public class IntegrationTestBase
    {
        protected readonly CustomWebApplicationFactory WebApplicationFactory;

        public IntegrationTestBase()
        {
            WebApplicationFactory = new CustomWebApplicationFactory();
        }

        public HttpClient GetClient() => WebApplicationFactory.CreateClient();
    }
}