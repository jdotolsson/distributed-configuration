using Microsoft.Extensions.Options;

namespace JESoft.Catalog.Api.ConfigurationTests
{
    public class ConfigurationValidationTest : IntegrationTestBase
    {
        private string ConfigurationErrorSource => $"Configuration is not valid. Configuration Source: {WebApplicationFactory.ConfigurationSource}";
        [Test]
        public void ValidateConfiguration()
        {
            try
            {
                GetClient();
            }
            catch (OptionsValidationException validationException)
            {
                PrintConfigurationException(validationException);
                Assert.Fail(ConfigurationErrorSource);
            }
            catch (AggregateException aggregateException) when (aggregateException.InnerExceptions.Any(ex => ex is OptionsValidationException))
            {
                foreach (var validationException in aggregateException.InnerExceptions.OfType<OptionsValidationException>())
                {
                    PrintConfigurationException(validationException);
                }
                Assert.Fail(ConfigurationErrorSource);
            }
            Assert.Pass();
        }

        private static void PrintConfigurationException(OptionsValidationException validationException)
        {
            var header = validationException.OptionsName == Options.DefaultName
                       ? validationException.OptionsType.Name
                       : $"{validationException.OptionsType.Name} with name '{validationException.OptionsName}'";
            TestContext.Error.WriteLine($"{header}");
            foreach (var error in validationException.Failures)
            {
                TestContext.Error.WriteLine($"{error}");
            }

            TestContext.Error.WriteLine("---");
        }
    }
}