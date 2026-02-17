using APIInterfaceTest.Models;
using System.Net;
using System.Net.Http.Json;

namespace APIInterfaceTest.EndpointConnections
{
    internal class ConfigurationEndpoints
    {
        public static async Task<List<Configuration>> retrieveConfigurations(HttpClient client)
        {
            HttpResponseMessage? response = null;
            List<Configuration> returnedConfigurations = new List<Configuration>();
            try
            {
                Console.WriteLine("Retrieving configurations");
                response = await client.GetAsync("configurations");
                if (response.IsSuccessStatusCode)
                {
                    string responseTest = await response.Content.ReadAsStringAsync();
                    returnedConfigurations = await response.Content.ReadFromJsonAsync<List<Configuration>>() ?? new List<Configuration>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving configurations: {ex.Message}");
            }

            return returnedConfigurations;
        }

        public static async Task<HttpResponseMessage> deleteConfiguration(HttpClient client, Configuration configuration)
        {
            Console.WriteLine("Deleting configuration");
            HttpResponseMessage response = await client.DeleteAsync($"/configurations/{configuration.Id}");
            Console.WriteLine($"Deletion result status code - {response.StatusCode}");
            return response;
        }

        public static async Task<HttpResponseMessage?> postConfiguration(HttpClient client, Configuration configuration)
        {
            HttpResponseMessage? response = null;
            try
            {
                Console.WriteLine("Posting configuration");
                response = await client.PostAsJsonAsync("configurations", configuration);
                response.EnsureSuccessStatusCode();
                Uri? creationReponse = response.Headers.Location;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting configuration - {ex.Message}");
            }

            return response;
        }

        public static async Task postConfigurations(HttpClient client, List<Configuration> items)
        {
            foreach (Configuration item in items)
            {
                await postConfiguration(client, item);
            }
        }

        public static async Task<HttpResponseMessage> updateConfiguration(HttpClient client, Configuration configuration)
        {
            Console.WriteLine("Updating configuration");
            HttpResponseMessage response = await client.PutAsJsonAsync($"configurations/{configuration.Id}", configuration);
            response.EnsureSuccessStatusCode();
            Console.WriteLine($"Update result status code - {response.StatusCode}");

            List<Configuration>? updatedConfigurations = null;
            try
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    updatedConfigurations = await response.Content.ReadFromJsonAsync<List<Configuration>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating configuration - {ex.Message}");
            }
            return response;
        }
    }
}
