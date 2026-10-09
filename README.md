# ASP.NET Core + SignalR Chat Backend

A minimal real-time public-chat backend using ASP.NET Core 8 and SignalR. It demonstrates hub-based messaging, online presence, typing indicators, server-side input validation, and a configurable browser-origin allow-list.

## Features

- SignalR hub mapped at `/chat`.
- Join/welcome, message, presence, leave, and typing events.
- Thread-safe in-memory connection presence.
- Display names limited to 32 characters and messages limited to 2,000 characters.
- A unique identifier for every published message, so clients can deduplicate individual messages safely.
- Configurable listener URL through `ASPNETCORE_URLS` and trusted frontend origins through `CHAT_ALLOWED_ORIGINS`.
- `GET /health` for a basic health check.

## Requirements

- .NET 8 SDK

## Run locally

From the repository root:

```bash
dotnet restore signalr-aspnetcore-public-chatroom.sln
dotnet run --project src/src.csproj
```

The default ASP.NET Core URL is usually `http://localhost:5000` when configured as shown below. The hub endpoint is `http://localhost:5000/chat`.

### Configuration

`CHAT_ALLOWED_ORIGINS` is a semicolon-separated list. The default development allow-list is:

```text
http://localhost:3000
http://127.0.0.1:3000
http://127.0.0.1:5500
```

PowerShell example:

```powershell
$env:ASPNETCORE_URLS = "http://localhost:5000"
$env:CHAT_ALLOWED_ORIGINS = "http://localhost:3000;http://127.0.0.1:3000"
dotnet run --project src/src.csproj
```

Configure only trusted frontend origins in deployed environments. CORS is not authentication. This sample does not implement user identity, authorization, persistence, or rate limiting.

## SignalR hub contract

### Client invokes

| Method | Argument |
| --- | --- |
| `UserJoin` | `string username` |
| `SendMessage` | `string message` |
| `TypingStart` | no arguments |
| `TypingStop` | no arguments |

### Server sends

| Event | Payload |
| --- | --- |
| `welcome` | `{ message, users: string[] }` |
| `user-joined` | `{ username, message, time }` |
| `user-left` | `{ username, message, time }` |
| `new-message` | `{ username, message, time, id }` |
| `online-users` | `string[]` |
| `user-typing` | `{ username, isTyping }` |

Timestamps are emitted as UTC ISO-8601 strings. Invalid input and sending a message before joining result in a SignalR hub error.

## Run tests

```bash
dotnet test signalr-aspnetcore-public-chatroom.sln
dotnet build signalr-aspnetcore-public-chatroom.sln --configuration Release
```

The test project covers display-name and message validation boundaries. GitHub Actions builds the solution and runs the tests on pushes and pull requests.

## Limitations

Presence is process-local and disappears on restart. Multiple server instances need a shared presence store and a supported SignalR scale-out service. Authentication, authorization, persistence, backpressure, and rate limiting are intentionally out of scope.
