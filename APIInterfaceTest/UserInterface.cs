using APIInterfaceTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static APIInterfaceTest.EndpointConnections.ItemEndpoints;

namespace APIInterfaceTest
{
    internal class UserInterface
    {
        public static async Task APIUserInput(HttpClient client)
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
            catch (Exception ex)
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
            catch (Exception ex)
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
    }
}
