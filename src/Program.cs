using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5000", "http://127.0.0.1:5500", "null")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors("AllowSpecific");
app.MapHub<ChatHub>("/chat");

app.Run();

public class ChatHub : Hub
{
    private static Dictionary<string, string> _users = new();

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"✅ New user connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public async Task UserJoin(string username)
    {
        _users[Context.ConnectionId] = username;
        
        await Clients.Others.SendAsync("user-joined", new
        {
            username = username,
            message = $"{username} joined the chat",
            time = DateTime.Now.ToString("HH:mm:ss")
        });

        await Clients.Caller.SendAsync("welcome", new
        {
            message = $"Welcome to the chatroom {username}!",
            users = _users.Values.ToList()
        });

        await Clients.All.SendAsync("online-users", _users.Values.ToList());
        
        Console.WriteLine($"👤 User joined: {username} (Total: {_users.Count})");
    }

    public async Task SendMessage(string message)
    {
        if (_users.TryGetValue(Context.ConnectionId, out string? username))
        {
            await Clients.All.SendAsync("new-message", new
            {
                username = username,
                message = message,
                time = DateTime.Now.ToString("HH:mm:ss"),
                id = Context.ConnectionId
            });
            Console.WriteLine($"💬 Message from {username}: {message}");
        }
    }

    public async Task TypingStart()
    {
        if (_users.TryGetValue(Context.ConnectionId, out string? username))
        {
            await Clients.Others.SendAsync("user-typing", new
            {
                username = username,
                isTyping = true
            });
        }
    }

    public async Task TypingStop()
    {
        if (_users.TryGetValue(Context.ConnectionId, out string? username))
        {
            await Clients.Others.SendAsync("user-typing", new
            {
                username = username,
                isTyping = false
            });
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (_users.TryGetValue(Context.ConnectionId, out string? username))
        {
            _users.Remove(Context.ConnectionId);
            
            await Clients.All.SendAsync("user-left", new
            {
                username = username,
                message = $"{username} left the chat",
                time = DateTime.Now.ToString("HH:mm:ss")
            });
            
            await Clients.All.SendAsync("online-users", _users.Values.ToList());
            
            Console.WriteLine($"❌ User left: {username} (Remaining: {_users.Count})");
        }
        await base.OnDisconnectedAsync(exception);
    }
}