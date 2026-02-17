using APIInterfaceTest.Models;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace APIInterfaceTest.EndpointConnections
{
    internal class CategoryEndpoints
    {
        public static async Task<List<Category>> retrieveCategories(HttpClient client)
        {
            HttpResponseMessage? response = null;
            List<Category> returnedCategories = new List<Category>();
            try
            {
                Console.WriteLine("Retrieving categories");
                response = await client.GetAsync("categories");
                if (response.IsSuccessStatusCode)
                {
                    string responseTest = await response.Content.ReadAsStringAsync();
                    returnedCategories = await response.Content.ReadFromJsonAsync<List<Category>>() ?? new List<Category>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
            }

            return returnedCategories;
        }

        public static async Task<List<Category>> retrieveCategoriesById(HttpClient client, int id)
        {
            HttpResponseMessage? response = null;
            List<Category> returnedCategories = new List<Category>();
            try
            {
                Console.WriteLine($"Retrieving categories with Id {id}");
                response = await client.GetAsync($"categories/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string responseTest = await response.Content.ReadAsStringAsync();
                    returnedCategories = await response.Content.ReadFromJsonAsync<List<Category>>() ?? new List<Category>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving categories: {ex.Message}");
            }

            return returnedCategories;
        }

        public static async Task<HttpResponseMessage> deleteCategory(HttpClient client, Category category)
        {
            HttpResponseMessage response = await client.DeleteAsync($"categories/{category.Id}");
            return response;
        }
    } 
}
