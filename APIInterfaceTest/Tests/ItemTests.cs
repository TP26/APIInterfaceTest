using APIInterfaceTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static APIInterfaceTest.EndpointConnections.ItemEndpoints;

namespace APIInterfaceTest.Tests
{
    public class ItemTests
    {
        public static async Task Tests(HttpClient client)
        {
            await DeleteAllItems(client);

            Console.WriteLine("\n==========\nCommencing Test 1\n==========\n");
            List<Item> items = getTestItems();
            //Post items
            await postItems(client, items);

            //Retrieve items
            List<Item> returnedItems = await retrieveItems(client);

            //Comparing returned items
            compareItems(items, returnedItems);

            Console.WriteLine("\n==========\nCommencing Test 2\n==========\n");
            //Remove item
            Item removedItem = items.TakeLast(1).First();
            items.Remove(removedItem);
            await deleteItem(client, removedItem);

            //Retrieve and compare items
            returnedItems = await retrieveItems(client);
            compareItems(items, returnedItems);

            Console.WriteLine("\n==========\nCommencing Test 3\n==========\n");
            //Update item
            items[2].Position = 15;
            Item updatedItem = items[2];
            await updateItem(client, updatedItem);

            //Retrieve and compare items
            returnedItems = await retrieveItems(client);
            compareItems(items, returnedItems);

            Console.WriteLine("\n==========\nCommencing Test 4\n==========\n");
            //Retrieve items by position
            int position = 5;
            List<Item> positionItemsAPI = await retrieveItemsByPosition(client, position);
            List<Item> positionItemsLocal = items.Where(item => item.Position == position).ToList();
            compareItems(positionItemsAPI, positionItemsLocal);

            //Remove all items
            returnedItems = await retrieveItems(client);

            foreach (Item item in returnedItems)
            {
                await deleteItem(client, item);
            }

            returnedItems = await retrieveItems(client);

            if (returnedItems.Count > 0)
            {
                Console.WriteLine("Error - could not delete all items.");
            }
        }

        public static bool compareItems(List<Item> itemListOne, List<Item> itemListTwo)
        {
            bool itemListsMatch = true;
            itemListOne = itemListOne.OrderBy(item => item.Id).ToList();
            itemListTwo = itemListTwo.OrderBy(item => item.Id).ToList();
            Console.WriteLine("Comparing item lists");
            if (itemListOne.Count == itemListTwo.Count)
            {
                for (int i = 0; i < itemListOne.Count; i++)
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
                        itemListsMatch = false;
                    }
                }
            }
            else
            {
                Console.WriteLine("Count of item lists do not match.");
            }

            return itemListsMatch;
        }

        private static async Task DeleteAllItems(HttpClient client)
        {
            //Get existing items
            List<Item> existingItems = await retrieveItems(client);
            List<int> uniqueItemIds = new List<int>();

            foreach(Item existingItem in existingItems)
            {
                if (!uniqueItemIds.Contains(existingItem.Id))
                {
                    uniqueItemIds.Add(existingItem.Id);
                }
            }

            foreach (int itemId in uniqueItemIds)
            {
                await deleteItem(client, itemId);
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
    }
}
