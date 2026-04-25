# MiniWebServer

A lightweight web server written in **C#** using `TcpListener`.  
It handles basic HTTP requests and responses without any external libraries.

## Features
- Serves HTML for `/`
- Returns JSON for `/api/users`
- Simple multi-threaded request handling

## How to Run
```bash
dotnet run
```

### Then open your browser at:

```text
http://localhost:8080
```

## Example
Accessing /api/users will respond with a JSON list of users.
