using Chat.Client.Hubs;

namespace Chat.Client;
public class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("What is your name:");
        var username = Console.ReadLine();

        var client = new ClientHub(username);
        client.ConnectAsync();

        while (true)
        {
            var message = Console.ReadLine();
            client.SendMessageAsync(message);
        }
    }
}