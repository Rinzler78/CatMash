using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatMash.ClientManager
{
    public class CatMashClient
    {
        public event EventHandler Closed;

        WebSocket WebSocket { get; }
        public CatMashClient(WebSocket webSocket)
        {
            WebSocket = webSocket;
        }

        public async Task Start()
        {
            Console.WriteLine($"Client Start");

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = null;

            do
            {
                result = await WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            } while (!result.CloseStatus.HasValue);

            await WebSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);

            Closed?.Invoke(this, EventArgs.Empty);
        }

        public async Task NotifyRate(string winnerId, string opponentId)
        {
            var request = Encoding.ASCII.GetBytes($"{winnerId}:{opponentId}");

            await WebSocket.SendAsync(new ArraySegment<byte>(request, 0, request.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
