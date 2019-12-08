using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace CatMash.ClientManager
{
    public class CatMashClientManager : ICatMashClientManager
    {
        readonly List<CatMashClient> CatMashClients = new List<CatMashClient>();

        public async Task NotifyRate(string winnerId, string opponentId)
        {
            Task[] tasks = null;

            lock (CatMashClients)
            {
                if (CatMashClients.Count == 0)
                    return;

                tasks = new Task[CatMashClients.Count];

                for (int i = 0; i < tasks.Length; ++i)
                {
                    var client = CatMashClients[i];
                    tasks[i] = client.NotifyRate(winnerId, opponentId);
                }
            }

            await Task.WhenAll(tasks);
        }

        public CatMashClient AddClient(WebSocket webSocket)
        {
            lock(CatMashClients)
            {
                var client = new CatMashClient(webSocket);

                client.Closed += Client_Closed;
                CatMashClients.Add(client);

                Debug.WriteLine($"Add client => {CatMashClients.Count}");

                return client;
            }
        }

        void Client_Closed(object sender, EventArgs e)
        {
            lock (CatMashClients)
            {
                var client = sender as CatMashClient;

                if (client != null)
                {
                    client.Closed -= Client_Closed;

                    CatMashClients.Remove(client);

                    Debug.WriteLine($"Remove client => {CatMashClients.Count}");
                }
            }
        }
    }
}
