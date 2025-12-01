using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

HttpClient client = initialiseClient();

Console.WriteLine("Enter execution option:");
Console.WriteLine("[1] - API tests");
Console.WriteLine("[2] - Manual input");
string option = Console.ReadLine() ?? "";

switch (option)
{
    case "1":
        await APITests(client);
        break;
    case "2":
        await APIUserInput(client);
        break;
}

static async Task<List<Item>> retrieveItems(HttpClient client)
{
    HttpResponseMessage? response = null;
    List<Item> returnedItems = new List<Item>();
    try
    {
        Console.WriteLine("Retrieving items");
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

static async Task<List<Item>> retrieveItemsByPosition(HttpClient client, int position)
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

static async Task<HttpResponseMessage?> postItem(HttpClient client, Item item)
{
    HttpResponseMessage? response = null;
    try
    {
        Console.WriteLine("Posting item");
        response = await client.PostAsJsonAsync("items", item);
        response.EnsureSuccessStatusCode();
        Uri? creationReponse = response.Headers.Location;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error posting item - {ex.Message}");
    }

    return response;
}

static async Task postItems(HttpClient client, List<Item> items)
{
    foreach (Item item in items)
    {
        await postItem(client, item);
    }
}

static async Task<HttpResponseMessage> updateItem(HttpClient client, Item item)
{
    Console.WriteLine("Updating item");
    HttpResponseMessage response = await client.PutAsJsonAsync($"items/{item.Id}", item);
    response.EnsureSuccessStatusCode();
    List<Item> updatedItems = null;

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

static async Task<HttpResponseMessage> deleteItem(HttpClient client, Item item)
{
    Console.WriteLine("Deleting item");
    HttpResponseMessage response = await client.DeleteAsync($"items/{item.Id}");
    HttpStatusCode deleteCodeResponse = response.StatusCode;
    return response;
}

static async Task<HttpResponseMessage> deleteItemById(HttpClient client, int id)
{
    Console.WriteLine("Deleting item");
    HttpResponseMessage response = await client.DeleteAsync($"items/{id}");
    HttpStatusCode deleteCodeResponse = response.StatusCode;
    return response;
}

static void compareItems(List<Item> itemListOne,  List<Item> itemListTwo)
{
    Console.WriteLine("Comparing item lists");
    if (itemListOne.Count == itemListTwo.Count)
    {
        for(int i = 0; i < itemListOne.Count; i++)
        {
            Item itemOne = itemListOne[i];
            Item itemTwo = itemListTwo[i];

            bool itemsMatch = true;

            if (itemOne.Id != itemTwo.Id)
            {
                itemsMatch = false;
                Console.WriteLine("Id does not match");
            }

            if (itemOne.Name != itemTwo.Name)
            {
                itemsMatch = false;
                Console.WriteLine("Name does not match");
            }

            if (itemOne.Position != itemTwo.Position)
            {
                itemsMatch = false;
                Console.WriteLine("Positions do not match");
            }

            if (!itemsMatch)
            {
                Console.WriteLine($"Item at index of {i} do not match");
            }
        }
    }
    else
    {
        Console.WriteLine("Count of item lists do not match.");
    }
}

static List<Item> getTestItems()
{
    return new List<Item>()
    {
        new Item() { Id = 1, Name = "ItemOne", Position = 2 },
        new Item() { Id = 2, Name = "ItemTwo", Position = 10 },
        new Item() { Id = 3, Name = "ItemThree", Position = 8 },
        new Item() { Id = 4, Name = "ItemFour", Position = 7 },
        new Item() { Id = 5, Name = "ItemFive", Position = 5 },
        new Item() { Id = 6, Name = "ItemSix", Position = 5 },
        new Item() { Id = 7, Name = "ItemSeven", Position = 5 }
    };
}

static HttpClient initialiseClient()
{
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5058/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return client;
}

static async Task APITests(HttpClient client)
{
    try
    {
        List<Item> items = getTestItems();
        //Post items
        await postItems(client, items);

        //Retrieve items
        List<Item> returnedItems = await retrieveItems(client);

        //Comparing returned items
        compareItems(items, returnedItems);

        //Remove item
        Item removedItem = items.TakeLast(1).First();
        items.Remove(removedItem);
        await deleteItem(client, removedItem);

        //Retrieve and compare items
        returnedItems = await retrieveItems(client);
        compareItems(items, returnedItems);

        //Update item
        items[2].Position = 15;
        Item updatedItem = items[2];
        await updateItem(client, updatedItem);

        //Retrieve and compare items
        returnedItems = await retrieveItems(client);
        compareItems(items, returnedItems);

        //Retrieve items by position
        int position = 5;
        List<Item> positionItemsAPI = await retrieveItemsByPosition(client, position);
        List<Item> positionItemsLocal = items.Where(item => item.Position == position).ToList();
        compareItems(positionItemsAPI, positionItemsLocal);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during execution - {ex.Message}");
    }
}

static Item getItemFromUser()
{
    Console.WriteLine("Item entry");
    Console.Write("Id: ");
    int id = Int32.Parse(Console.ReadLine() ?? "");
    Console.Write("Name: ");
    string name = Console.ReadLine() ?? "";
    Console.Write("Position: ");
    int position = Int32.Parse(Console.ReadLine() ?? "");

    return new Item() { Id = id, Name = name, Position = position };
}

static async Task postItemFromUser(HttpClient client)
{
    try
    {
        Item item = getItemFromUser();
        await postItem(client, item);
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error attempting to post item: {ex.Message}");
    }
}

static void displayItems(List<Item> items)
{
    foreach (Item item in items)
    {
        Console.WriteLine($"Id: {item.Id} Name: {item.Name} Position: {item.Position}");
    }
}

static async Task retrieveItemForUser(HttpClient client)
{
    try
    {
        Console.WriteLine("Retrieving items");
        Console.WriteLine("[1] - Retrieve all items");
        Console.WriteLine("[2] - Retrieve items by position");
        string option = Console.ReadLine() ?? "";

        switch (option)
        {
            case "1":
                await retrieveAllItemsForUser(client);
                break;
            case "2":
                await retrieveItemByPositionFromUser(client);
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error retrieving items: {ex.Message}");
    }
}

static async Task retrieveItemByPositionFromUser(HttpClient client)
{
    try
    {
        Console.WriteLine("Retrieving items by position");
        Console.Write("Enter position: ");
        int position = Int32.Parse(Console.ReadLine() ?? "");
        List<Item> retrievedItems = await retrieveItemsByPosition(client, position);
        displayItems(retrievedItems);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error retrieving items by position: {ex.Message}");
    }
}

static async Task retrieveAllItemsForUser(HttpClient client)
{
    try
    {
        Console.WriteLine("Retrieving all items");
        List<Item> retrievedItems = await retrieveItems(client);

        displayItems(retrievedItems);
    }
    catch(Exception ex)
    {
        Console.WriteLine($"Error retrieving all items: {ex.Message}");
    }
}

static async Task deleteItemFromUser(HttpClient client)
{
    try
    {
        Console.WriteLine("Deleting item");
        Console.Write("Enter item Id: ");
        int id = Int32.Parse(Console.ReadLine() ?? string.Empty);
        await deleteItemById(client, id);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error deleting item: {ex.Message}");
    }
}

static async Task updateItemFromUser(HttpClient client)
{
    try
    {
        Console.WriteLine("Updating item");
        Item item = getItemFromUser();
        await updateItem(client, item);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error attempting to update item: {ex.Message}");
    }
}

static async Task APIUserInput(HttpClient client)
{
    Console.WriteLine("Manual user input");
    try
    {
        bool getUserInput = true;

        while (getUserInput)
        {
            Console.WriteLine("Input options: ");
            Console.WriteLine("[1] - Post item: ");
            Console.WriteLine("[2] - Retrieve items: ");
            Console.WriteLine("[3] - Delete item: ");
            Console.WriteLine("[4] - Update item: ");
            Console.WriteLine("Exit");

            string input = Console.ReadLine() ?? "";

            switch (input)
            {
                case "1":
                    await postItemFromUser(client);
                    break;
                case "2":
                    await retrieveItemForUser(client);
                    break;
                case "3":
                    await deleteItemFromUser(client);
                    break;
                case "4":
                    await updateItemFromUser(client);
                    break;
            }

            if (input.ToLower().Contains("exit"))
            {
                getUserInput = false;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during execution - {ex.Message}");
    }
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Position { get; set; }
}