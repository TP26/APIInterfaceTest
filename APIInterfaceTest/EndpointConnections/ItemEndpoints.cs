using APIInterfaceTest.Models;
using System.Net;
using System.Net.Http.Json;

namespace APIInterfaceTest.EndpointConnections
{
    internal class ItemEndpoints
    {
        public static async Task<List<Item>> retrieveItems(HttpClient client)
        {
            HttpResponseMessage? response = null;
            List<Item> returnedItems = new List<Item>();
            try
            {
                response = await client.GetAsync("items");
                if (response.IsSuccessStatusCode)
                {
                    string responseTest = await response.Content.ReadAsStringAsync();
                    returnedItems = await response.Content.ReadFromJsonAsync<List<Item>>() ?? new List<Item>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving items: {ex.Message}");
            }

            return returnedItems;
        }

        public static async Task<List<Item>> retrieveItemsByPosition(HttpClient client, int position)
        {
            HttpResponseMessage? response = null;
            List<Item> returnedItems = new List<Item>();
            try
            {
                Console.WriteLine($"Retrieving items filtering to position {position}");
                response = await client.GetAsync($"items/find/{position}");
                if (response.IsSuccessStatusCode)
                {
                    string responseTest = await response.Content.ReadAsStringAsync();
                    returnedItems = await response.Content.ReadFromJsonAsync<List<Item>>() ?? new List<Item>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving items: {ex.Message}");
            }

            return returnedItems;
        }

        public static async Task<HttpResponseMessage?> postItem(HttpClient client, Item item)
        {
            HttpResponseMessage? response = null;
            try
            {
                response = await client.PostAsJsonAsync("items", item);
                string responseString = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
                Uri? creationReponse = response.Headers.Location;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error posting item - {ex.Message}");
            }

            return response;
        }

        public static async Task postItems(HttpClient client, List<Item> items)
        {
            foreach (Item item in items)
            {
                await postItem(client, item);
            }
        }

        public static async Task<HttpResponseMessage> updateItem(HttpClient client, Item item)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync($"items/{item.Id}", item);
            response.EnsureSuccessStatusCode();
            List<Item>? updatedItems = null;

            try
            {
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    string responseText = await response.Content.ReadAsStringAsync();
                    updatedItems = await response.Content.ReadFromJsonAsync<List<Item>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating item - {ex.Message}");
            }

            return response;
        }

        public static async Task<HttpResponseMessage> deleteItem(HttpClient client, Item item)
        {
            HttpResponseMessage response = await client.DeleteAsync($"items/{item.Id}");
            return response;
        }

        public static async Task<HttpResponseMessage> deleteItem(HttpClient client, int itemId)
        {
            HttpResponseMessage response = await client.DeleteAsync($"items/{itemId}");
            return response;
        }

        public static async Task<HttpResponseMessage> deleteItemById(HttpClient client, int id)
        {
            HttpResponseMessage response = await client.DeleteAsync($"items/{id}");
            HttpStatusCode deleteCodeResponse = response.StatusCode;
            return response;
        }
    }
}
