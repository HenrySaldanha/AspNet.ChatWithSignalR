This repository aims to present a simple implementation of the **SignalR**

# 1. Server
## Settings

In my **Startup** class I added the following code snippets:
The configuration below adds **SignalR** to our project and configures our hub's routing.

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSignalR();
        ...
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        ...
        app.UseEndpoints(endpoints => { endpoints.MapHub<ChatHub>("/chat"); });
    }
    
## ChatHub class implementation
The **ChatHub** class read creates the communication between the client and the server.

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

    public class Message
    {
        public Message(string userName, string text)
        {
            UserName = userName;
            Text = text;
        }

        public string UserName { get; set; }
        public string Text { get; set; }
    }


## This project was built with
* [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [MediatR](https://www.nuget.org/packages/MediatR/)
* [Swagger](https://swagger.io/)
* ~~coffee, pizza and late nights~~

## My contacts
* [LinkedIn](https://www.linkedin.com/in/henry-saldanha-3b930b98/)