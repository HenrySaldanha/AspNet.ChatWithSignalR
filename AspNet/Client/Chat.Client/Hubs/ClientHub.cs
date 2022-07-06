using Chat.Client.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chat.Client.Hubs;
public class ClientHub
{
    private readonly string UserName;
    private HubConnection Connection { get; set; }

    public ClientHub(string userName)
    {
        UserName = userName;
    }

    public async Task ConnectAsync()
    {
        Connection = new HubConnectionBuilder().WithUrl("https://localhost:7048/chat").Build();

        ConfigureNewUserMethod();
        ConfigureNewMessageMethod();
        ConfigurePreviousMessagesMethod();

        try
        {
            await Connection.StartAsync();
            await Connection.SendAsync("newUser", UserName, Connection.ConnectionId);
        }
        catch
        {
            Console.WriteLine($"Ocorreu um erro ao se conectar!");
        }
    }

    private void ConfigureNewUserMethod()
    {
        Connection.On<string>("newUser", user =>
        {
            var message = UserName == user ? "You entered the room" : $"{user}just entered";
            Console.WriteLine(message);
        });
    }

    private void ConfigureNewMessageMethod()
    {
        Connection.On<string, string>("newMessage", (user, message) =>
        {
            if (user != UserName)
                Console.WriteLine($"{user}: {message}");
        });
    }

    private void ConfigurePreviousMessagesMethod()
    {
        Connection.On<List<Message>>("previousMessages", (messages) =>
        {
            foreach (var item in messages)
                Console.WriteLine($"{item.UserName}: {item.Text}");
        });
    }


    public async Task SendMessageAsync(string message)
    {
        try
        {
            await Connection.SendAsync("newMessage", UserName, message);
        }
        catch
        {
            Console.WriteLine($"Ocorreu um erro ao enviar a mensagem");
        }
    }
}