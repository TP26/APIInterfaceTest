using APIInterfaceTest.Models;
using static APIInterfaceTest.EndpointConnections.CategoryEndpoints;
using static APIInterfaceTest.EndpointConnections.ConfigurationEndpoints;
using static APIInterfaceTest.EndpointConnections.ItemEndpoints;
using static APIInterfaceTest.Tests.ItemTests;

namespace APIInterfaceTest.Tests
{
    internal class Tests
    {
        public static async Task APITests(HttpClient client)
        {
            try
            {
                await ItemTests.Tests(client);
                await ConfigurationTests.Tests(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during execution - {ex.Message}");
            }
        }
    }
}
