# SignalR ASP.NET Core Public Chatroom

A real-time public chatroom backend built with ASP.NET Core 8 and SignalR. Supports online users list, typing indicators, join/leave notifications, and CORS enabled for cross-origin requests.

## Features

- ✅ Real-time messaging with timestamps
- ✅ Online users list with live updates
- ✅ Join and leave notifications
- ✅ Typing indicators
- ✅ Personal welcome message for new users
- ✅ CORS enabled for any client
- ✅ Lightweight and fast

## Tech Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| .NET | 8.0 | Runtime |
| ASP.NET Core | 8.0 | Web Framework |
| SignalR | 8.0 | Real-time Communication |
| C# | 12.0 | Programming Language |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Installation

### 1. Clone the repository

```bash
git clone https://github.com/peymanpro/signalr-aspnetcore-public-chatroom.git
cd signalr-aspnetcore-public-chatroom/src

Run the application

dotnet run


The server will start at: http://localhost:5000

SignalR Events
Client → Server (Invoke)
Event	Description	Payload
UserJoin	User joins the chat	string username
SendMessage	Send a message to all users	object { message: string }
TypingStart	User starts typing	(empty)
TypingStop	User stops typing	(empty)
Server → Client (On)
Event	Description	Payload
welcome	Personal welcome message	{ message, users: [] }
user-joined	New user joined the chat	{ username, message, time }
user-left	User left the chat	{ username, message, time }
new-message	New chat message	{ username, message, time, id }
user-typing	Typing status	{ username, isTyping }
online-users	Current online users list	[username1, username2]



