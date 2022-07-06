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

# 2. Client

For the client I created a simple console application.
The first step is to read the username and connect to the server to be able to send and receive messages. Then the app is in a loop waiting for the user to send something to the server.

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
    
In my ClientHub class I set the same server methods: newUser, newMessage and previousMessages. And for each method something is written to the console.

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

    public class Message
    {
        public string UserName { get; set; }
        public string Text { get; set; }
    }


Pay attention: To test, you need to build the server first and then the client.

## This project was built with
* [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
* [SignalR](https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-6.0&tabs=visual-studio)
* [Serilog](https://serilog.net/)

## My contacts
* [LinkedIn](https://www.linkedin.com/in/henry-saldanha-3b930b98/)