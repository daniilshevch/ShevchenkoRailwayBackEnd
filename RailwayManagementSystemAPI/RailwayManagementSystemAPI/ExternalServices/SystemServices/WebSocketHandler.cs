using System.Net.WebSockets;
using System.Text;

public static class WebSocketHandler
{
    private static readonly List<WebSocket> clients = new List<WebSocket>();
    private static readonly object sync = new object();

    public static async Task Handle(HttpContext context, WebSocket socket)
    {
        lock (sync)
        {
            clients.Add(socket);
        }

        byte[] buffer = new byte[1024];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    lock (sync)
                    {
                        clients.Remove(socket);
                    }
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                }
            }
        }
        catch
        {
            lock (sync)
            {
                clients.Remove(socket);
            }
        }
    }

    public static async Task BroadcastAsync(string message)
    {
        byte[] encoded = Encoding.UTF8.GetBytes(message);
        ArraySegment<byte> buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);

        List<WebSocket> snapshot;
        lock (sync)
        {
            snapshot = clients
                .Where(s => s != null && s.State == WebSocketState.Open)
                .ToList();
        }

        foreach (WebSocket socket in snapshot)
        {
            try
            {
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch
            {
                
            }
        }
    }
}
