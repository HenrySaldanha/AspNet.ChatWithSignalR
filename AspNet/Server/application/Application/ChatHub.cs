using Domain;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace Application;
public class ChatHub : Hub
{
    public static List<Message> Messages { get; set; }

    public ChatHub()
    {
        Messages ??= new List<Message>();
    }

    public void NewMessage(string userName, string message)
    {
        Log.Information($"New message recived from {userName}: {message}");

        Clients.All.SendAsync("newMessage", userName, message);

        Messages.Add(new Message(userName, message));
    }

    public void NewUser(string userName, string connectionId)
    {
        Clients.Client(connectionId).SendAsync("previousMessages", Messages);

        Log.Information($"New user has joined {userName}");
        Clients.All.SendAsync("newUser", userName);
    }
}