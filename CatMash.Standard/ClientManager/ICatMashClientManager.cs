using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace CatMash.ClientManager
{
    public interface ICatMashClientManager
    {
        Task NotifyRate(string winnerId, string opponentId);
        CatMashClient AddClient(WebSocket webSocket);
    }
}
