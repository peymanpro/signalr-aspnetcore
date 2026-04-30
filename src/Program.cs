using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();
builder.Services.AddCors();

var app = builder.Build();

app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapHub<ChatHub>("/chat");

app.Run();

public class ChatHub : Hub
{
    private static Dictionary<string, string> _users = new();

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"New user connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public async Task UserJoin(string username)
    {
        _users[Context.ConnectionId] = username;
        
        await Clients.Others.SendAsync("user-joined", new
        {
            username = username,
            message = $"{username} joined the chat",
            time = DateTime.Now
        });

        await Clients.Caller.SendAsync("welcome", new
        {
            message = $"Welcome to the chatroom {username}!",
            users = _users.Values.ToList()
        });

        await SendOnlineUsers();
    }

    public async Task SendMessage(object data)
    {
        if (_users.TryGetValue(Context.ConnectionId, out string? username))
        {
            var message = data.ToString();
            await Clients.All.SendAsync("new-message", new
            {
                username = username,
                message = message,
                time = DateTime.Now,
                id = Context.ConnectionId
            });
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
                time = DateTime.Now
            });
            
            await SendOnlineUsers();
        }
        await base.OnDisconnectedAsync(exception);
    }

    private async Task SendOnlineUsers()
    {
        await Clients.All.SendAsync("online-users", _users.Values.ToList());
    }
}