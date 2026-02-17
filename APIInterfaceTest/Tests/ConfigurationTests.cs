using APIInterfaceTest.Models;
using static APIInterfaceTest.EndpointConnections.CategoryEndpoints;
using static APIInterfaceTest.EndpointConnections.ConfigurationEndpoints;
using static APIInterfaceTest.EndpointConnections.ItemEndpoints;
using static APIInterfaceTest.Tests.ItemTests;

namespace APIInterfaceTest.Tests
{
    internal class ConfigurationTests
    {
        public static async Task Tests(HttpClient client)
        {
            //Remove existing configurations
            await DeleteAllConfigurations(client);
            await DeleteAllCategories(client);

            List<Configuration> configurations = getTestConfigurations2();

            //Post Configurations
            await postConfigurations(client, configurations);

            //Retrieve Configurations
            List<Configuration> retrievedConfigurations = await retrieveConfigurations(client);

            //Compare configurations
            compareConfigurationLists(configurations, retrievedConfigurations);

            //Retrieve items and check posted items inside configurations have been created
            List<Item> postedItems = new List<Item>();
            List<int> postedItemIds = new List<int>();

            foreach (Configuration configuration in configurations)
            {
                foreach (Item postedItem in configuration.items)
                {
                    if (!postedItemIds.Contains(postedItem.Id))
                    {
                        postedItemIds.Add(postedItem.Id);
                        postedItems.Add(postedItem);
                    }
                }
            }

            List<Item> retrievedItems = await retrieveItems(client);
            if (compareItems(postedItems, retrievedItems))
            {
                Console.WriteLine("Items posted inside configurations have been created.");
            }
            else
            {
                Console.WriteLine("Error: Items posted inside configurations have not been created.");
            }

            //Check categories posted inside configurations have been created
            List<Category> postedCategories = new List<Category>();
            List<int> postedCategoryIds = new List<int>();

            foreach (Configuration configuration in configurations)
            {
                if (!postedCategoryIds.Contains(configuration.category.Id))
                {
                    postedCategories.Add(configuration.category);
                    postedCategoryIds.Add(configuration.category.Id);
                }
            }

            List<Category> retrievedCategories = await retrieveCategories(client);
            if (compareCategories(postedCategories, retrievedCategories))
            {
                Console.WriteLine("Categories posted inside configurations have been created.");
            }
            else
            {
                Console.WriteLine("Error: Categories posted inside configurations have not been created.");
            }

            //Update configuration
            Console.WriteLine("Beginning update configuration test");
            Configuration updatedConfiguration = configurations[0];
            updatedConfiguration.category = new Category() { Id = 15, Name = "Updated Category" };
            updatedConfiguration.coOrd = new CoOrdinates() { Id = 15, X = 100, Y = 100 };
            updatedConfiguration.items = new List<Item>() { new Item() { Id = 15, Name = "Updated item", Position = 15 } };

            await updateConfiguration(client, updatedConfiguration);
            retrievedConfigurations = await retrieveConfigurations(client);
            compareConfigurationLists(configurations, retrievedConfigurations);
        }

        private static void compareConfigurationLists(List<Configuration> configurations, List<Configuration> retrievedConfigurations)
        {
            //Compare list counts
            if (configurations.Count != retrievedConfigurations.Count)
            {
                Console.WriteLine("Number of retrieved configurations does not match number of configurations posted");
            }
            else
            {
                //Check all configuration Ids are present
                List<int> configurationIds = new List<int>();

                foreach (Configuration configuration in configurations)
                {
                    configurationIds.Add(configuration.Id);
                }

                foreach (int configurationId in configurationIds)
                {
                    bool configurationWithIdFound = false;

                    foreach (Configuration retrievedConfiguration in retrievedConfigurations)
                    {
                        if (retrievedConfiguration.Id == configurationId)
                        {
                            configurationWithIdFound = true;
                        }
                    }

                    if (!configurationWithIdFound)
                    {
                        Console.WriteLine($"Error - Configuration with id {configurationId} was not found.");
                    }
                }

                //Compare each configuration
                foreach (int configId in configurationIds)
                {
                    Configuration? sentConfiguration = null;
                    Configuration? retrievedConfiguration = null;

                    foreach (Configuration configuration in configurations)
                    {
                        if (configuration.Id == configId)
                        {
                            sentConfiguration = configuration;
                        }
                    }

                    foreach (Configuration configuration in retrievedConfigurations)
                    {
                        if (configuration.Id == configId)
                        {
                            retrievedConfiguration = configuration;
                        }
                    }

                    if (sentConfiguration != null && retrievedConfiguration != null)
                    {
                        if (!compareConfigurations(sentConfiguration, retrievedConfiguration))
                        {
                            Console.WriteLine("\nConfigurations do not match, displaying both:");
                            Console.WriteLine("Sent configuration:");
                            displayConfiguration(sentConfiguration);
                            Console.WriteLine("\nRetrieved configuration:");
                            displayConfiguration(retrievedConfiguration);
                        }
                    }
                    else
                    {
                        throw new Exception("Configuration not found");
                    }
                }
            }
        }

        private static async Task DeleteAllConfigurations(HttpClient client)
        {
            //Get existing configurations
            List<Configuration> existingConfigurations = await retrieveConfigurations(client);
            foreach (Configuration configuration in existingConfigurations)
            {
                await deleteConfiguration(client, configuration);
            }
        }

        private static async Task DeleteAllCategories(HttpClient client)
        {
            //Get existing categories
            List<Category> existingCategories = await retrieveCategories(client);
            foreach(Category category in existingCategories)
            {
                await deleteCategory(client, category);
            }
        }

        static List<Configuration> getTestConfigurations()
        {
            return new List<Configuration>()
            {
                new Configuration() {
                    Id = 1,
                    category = new Category() { Id = 1, Name = "Category One"},
                    items = new List<Item>()
                    {
                        new Item() { Id = 1, Name = "Brick", Position = 5},
                        new Item() { Id = 2, Name = "Wall", Position = 6 },
                        new Item() { Id = 3, Name = "Stone", Position = 10 }
                    },
                    coOrd = new CoOrdinates() { Id = 1, X = 2, Y = 4 }
                },
                new Configuration() {
                    Id = 2,
                    category = new Category() { Id = 2, Name = "Category Two"},
                    items = new List<Item>()
                    {
                        new Item() { Id = 1, Name = "Brick", Position = 5},
                        new Item() { Id = 4, Name = "Rock", Position = 2 },
                        new Item() { Id = 5, Name = "Sand", Position = 11 }
                    },
                    coOrd = new CoOrdinates() { Id = 2, X = 5, Y = 2 }
                },
                new Configuration() {
                    Id = 3,
                    category = new Category() { Id = 1, Name = "Category One"},
                    items = new List<Item>()
                    {
                        new Item() { Id = 6, Name = "Mortar", Position = 1},
                        new Item() { Id = 7, Name = "Gate", Position = 1 },
                        new Item() { Id = 8, Name = "Post", Position = 1 }
                    },
                    coOrd = new CoOrdinates() { Id = 3, X = 1, Y = 1 }
                }
            };
        }


        static List<Configuration> getTestConfigurations2()
        {
            return new List<Configuration>()
            {
                new Configuration() {
                    Id = 1,
                    category = new Category() { Id = 1, Name = "Category One"},
                    items = new List<Item>()
                    {
                        new Item() { Id = 1, Name = "Brick", Position = 5},
                    },
                    coOrd = new CoOrdinates() { Id = 1, X = 2, Y = 4 }
                },
                new Configuration() {
                    Id = 2,
                    category = new Category() { Id = 2, Name = "Category Two"},
                    items = new List<Item>()
                    {
                        new Item() { Id = 1, Name = "Brick", Position = 5},
                    },
                    coOrd = new CoOrdinates() { Id = 2, X = 5, Y = 2 }
                },
                new Configuration() {
                    Id = 3,
                    category = new Category() { Id = 1, Name = "Category One"},
                    items = new List<Item>()
                    {
                        new Item() { Id = 6, Name = "Mortar", Position = 1},
                    },
                    coOrd = new CoOrdinates() { Id = 3, X = 1, Y = 1 }
                }
            };
        }

        static bool compareConfigurations(Configuration configurationOne, Configuration configurationTwo)
        {
            bool configurationsMatch = true;

            //Compare Ids
            if (configurationOne.Id != configurationTwo.Id)
            {
                Console.WriteLine($"Configuration Ids do not match {configurationOne.Id} and {configurationTwo.Id}");
                return false;
            }

            //Compare Items
            if (!compareItems(configurationOne.items, configurationTwo.items))
            {
                return false;
            }

            //Compare Categories
            if (!compareTwoCategories(configurationOne.category, configurationTwo.category))
            {
                return false;
            }

            //Compare CoOrdinates
            if (!compareTwoCoOrdinates(configurationOne.coOrd, configurationTwo.coOrd))
            {
                return false;
            }

            return configurationsMatch;
        }

        public static bool compareCategories(List<Category> categoryListOne, List<Category> categoryListTwo)
        {
            bool categoryListsMatch = true;
            categoryListOne = categoryListOne.OrderBy(category => category.Id).ToList();
            categoryListTwo = categoryListTwo.OrderBy(category => category.Id).ToList();
            Console.WriteLine("Comparing category lists");
            if (categoryListOne.Count == categoryListTwo.Count)
            {
                for (int i = 0; i < categoryListOne.Count; i++)
                {
                    if (!compareTwoCategories(categoryListOne[i], categoryListTwo[i]))
                    {
                        categoryListsMatch = false;
                    }

                }
            }
            else
            {
                Console.WriteLine("Count of category lists do not match.");
                categoryListsMatch = false;
            }

            return categoryListsMatch;
        }

        static bool compareTwoCategories(Category categoryOne, Category categoryTwo)
        {
            bool categoriesMatch = true;

            if (categoryOne.Id != categoryTwo.Id)
            {
                Console.WriteLine($"Category Ids do not match - {categoryOne.Id} & {categoryTwo.Id}");
                return false;
            }

            if (categoryOne.Name != categoryTwo.Name)
            {
                Console.WriteLine($"Category names do not match - {categoryOne.Name} & {categoryTwo.Name}");
                return false;
            }

            return categoriesMatch;
        }

        static bool compareTwoCoOrdinates(CoOrdinates coOrd1,  CoOrdinates coOrd2)
        {
            if (coOrd1.Id != coOrd2.Id)
            {
                Console.WriteLine($"Co-ordinate Ids do not match - {coOrd1.Id} & {coOrd2.Id}");
                return false;
            }

            if (coOrd1.X != coOrd2.X)
            {
                Console.WriteLine($"Co-ordinate X do not match - {coOrd1.X} & {coOrd2.X}");
                return false;
            }

            if (coOrd1.Y != coOrd2.Y)
            {
                Console.WriteLine($"Co-ordinate Y do not match - {coOrd1.Y} & {coOrd2.Y}");
                return false;
            }

            return true;
        }

        static void displayConfiguration(Configuration configuration)
        {
            Console.WriteLine("\nDisplaying configuration:");
            Console.WriteLine($"Id: {configuration.Id}");
            Console.WriteLine($"Category: Id: {configuration.category.Id}, name: {configuration.category.Name}");
            Console.WriteLine($"Co-Ordinates: Id: {configuration.coOrd.Id}, X: {configuration.coOrd.X}, Y: {configuration.coOrd.Y}");
            Console.WriteLine("Items:");
            foreach(Item item in configuration.items)
            {
                Console.WriteLine($"Id: {item.Id}, name: {item.Name}, position: {item.Position}");
            }
        }
    }
}
