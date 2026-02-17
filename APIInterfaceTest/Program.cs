using System.Net.Http.Headers;
using static APIInterfaceTest.Tests.Tests;
using static APIInterfaceTest.UserInterface;

HttpClient client = initialiseClient();

bool keepMenuOpen = true;

while (keepMenuOpen)
{
    Console.WriteLine("Enter execution option:");
    Console.WriteLine("[1] - API tests");
    Console.WriteLine("[2] - Manual input");
    Console.WriteLine("[3] - Exit");
    string option = Console.ReadLine() ?? "";

    switch (option)
    {
        case "1":
            await APITests(client);
            break;
        case "2":
            await APIUserInput(client);
            break;
        default:
            keepMenuOpen = false;
            break;
    }
}

static HttpClient initialiseClient()
{
    HttpClient client = new HttpClient();
    client.BaseAddress = new Uri("http://localhost:5058/");
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    return client;
}