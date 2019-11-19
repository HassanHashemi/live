using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;

namespace Live.Hub
{
    internal sealed class SocketCollection
    {
        private readonly Dictionary<ClientInfo, List<WebSocket>> _clients
            = new Dictionary<ClientInfo, List<WebSocket>>();

        public IEnumerable<WebSocket> GetConnections(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return _clients
                .Where(c => c.Key.UserId == userId)
                .SelectMany(kvp => kvp.Value);
        }

        public void Remove(ClientInfo client, WebSocket connection)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            var current = _clients.FirstOrDefault(c => c.Key == client);

            if (current.Key == null)
            {
                return;
            }

            current.Value.Remove(connection);

            if (current.Value.Count == 0)
            {
                _clients.Remove(current.Key);
            }
        }

        public void AddConnection(ClientInfo client, WebSocket connection)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var current = _clients.FirstOrDefault(c => c.Key == client);

            // user is not connected
            if (current.Key == null)
            {
                _clients.Add(client, new List<WebSocket>() { connection });
            }
            // user is connected
            else
            {
                current.Value.Add(connection);
            }
        }
    }
}
