using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace MiniWebServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Any;
            int port = 8080;
            TcpListener server = new TcpListener(ip, port);

            server.Start();
            Console.WriteLine($"Server is running on http://localhost:{port}");
            Console.WriteLine("Press any key to stop...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread thread = new Thread(() => HandleClient(client));
                thread.Start();
            }



        }

        static void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0) return;

                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                string[] requestLines = request.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                if (requestLines.Length == 0) return;

                string firstLine = requestLines[0];
                string[] parts = firstLine.Split(' ');
                string method = parts[0];
                string path = parts.Length > 1 ? parts[1] : "/";

                Console.WriteLine($"Received: {method} {path}");

                string responseBody = "";
                string contentType = "text/html";

                if (path == "/")
                {
                    responseBody = "<h1>Welcome to Mini Web Server!</h1><p><a href='/api/users'>Go to Users API</a></p>";
                }
                else if (path.StartsWith("/api/users"))
                {
                    var users = new[]
                    {
                        new { Id = 1, Name = "Hamed", Email = "hamed@gmail.com" },
                        new { Id = 2, Name = "Ali", Email = "ali@gmail.com" }
                    };

                    var options = new JsonSerializerOptions { WriteIndented = true };
                    responseBody = JsonSerializer.Serialize(users, options);

                    contentType = "application/json";
                }
                else
                {
                    responseBody = "<h1>404 Not Found</h1>";
                }

                byte[] responseBytes = Encoding.UTF8.GetBytes(responseBody);

                string headers = $"HTTP/1.1 200 OK\r\n" +
                                 $"Content-Type: {contentType}\r\n" +
                                 $"Content-Length: {responseBytes.Length}\r\n" +
                                 $"Connection: close\r\n" +
                                 $"\r\n";

                byte[] headerBytes = Encoding.UTF8.GetBytes(headers);

                stream.Write(headerBytes, 0, headerBytes.Length);
                stream.Write(responseBytes, 0, responseBytes.Length);

                Console.WriteLine("Response sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}