using System.Threading;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.WebSockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Kraken.Api.Sensor.Services
{
    /// <summary>
    /// The websocket service.
    /// </summary>
    public class WebSocketService
    {
        private IList<WebSocket> _clients;

        /// <summary>
        /// Creates a new websocket service.
        /// </summary>
        public WebSocketService()
        {
            _clients = new List<WebSocket>();
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        /// <param name="data">The data to transmit.</param>
        /// <typeparam name="T">The data type.</typeparam>
        public async void NotifyClients<T>(T data)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            string json = JsonConvert.SerializeObject(data, serializerSettings);
            byte[] bytes = Encoding.ASCII.GetBytes(json);
            var segment = new ArraySegment<byte>(bytes, 0, bytes.Length);

            RefreshClientList();
            foreach (WebSocket client in _clients.Where(socket => socket.State == WebSocketState.Open))
            {
                await client.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        /// <summary>
        /// Refreshes the list of clients.
        /// </summary>
        private void RefreshClientList()
        {
            _clients = _clients.Where(socket => socket.State != WebSocketState.Closed).ToList();
        }

        /// <summary>
        /// Adds a client to the list.
        /// </summary>
        /// <param name="webSocket">The websocket connection.</param>
        public void AddClient(WebSocket webSocket)
        {
            _clients.Add(webSocket);
        }
    }
}