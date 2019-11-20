﻿using Live.Backplane;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class SocketServiceHost : Microsoft.Extensions.Hosting.IHostedService
    {
        private readonly SocketServer _socketServer;
        private readonly IBackplaine _backplane;
        private readonly ILogger<SocketServiceHost> _logger;

        public SocketServiceHost(SocketServer socketServer, IBackplaine backplane, ILogger<SocketServiceHost> logger)
        {
            _socketServer = socketServer;
            _backplane = backplane;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _socketServer.ClientConnected += SocketServer_ClientConnected;
            _socketServer.TextReceived += SocketServer_TextReceived;
            _socketServer.ClientDisconnected += SocketServer_ClientDisconnected;
            _socketServer.ClientHeartBeat += SocketServer_ClientHeartBeat;

            _backplane.MessageReceived += Backplane_MessageReceived;

            await _backplane.Connect();
        }

        private async void Backplane_MessageReceived(object sender, BackplaneMessageReceivedArgs e)
        {
            try
            {
                using var cts = new CancellationTokenSource();
                cts.CancelAfter(3000);

                await _socketServer.SendJson(e.Message.UserId,
                    e.Message.Message,
                    cts.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error pushing to {e.Message.UserId}, info: {ex.Message}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void SocketServer_ClientHeartBeat(object sender, HeartBeatEventArgs e)
        {
            _logger.LogInformation($"{e.Client.UserId} from {e.Client.MerchantId} Hearbeat");
        }

        private void SocketServer_ClientConnected(object sender, ClientConnectedEventArgs e)
        {
            _logger.LogInformation($"{e.Client.UserId} from {e.Client.MerchantId} Connected");

            foreach (var item in _socketServer.ConnectedClients)
            {
                _logger.LogInformation($"{e.Client.UserId} from {e.Client.MerchantId} : {item.Value.Count}");
            }
        }

        private void SocketServer_ClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            _logger.LogInformation($"{e.Client.UserId} from {e.Client.MerchantId} disconnected");

            foreach (var item in _socketServer.ConnectedClients)
            {
                _logger.LogInformation($"{e.Client.UserId} from {e.Client.MerchantId} : {item.Value.Count}");
            }
        }

        private async void SocketServer_TextReceived(object sender, MessageReceivedArgs<string> e)
        {
            foreach (var item in _socketServer.ConnectedClients)
            {
                await _socketServer.SendString(item.Key.UserId, $"{e.Message} {e.Client.UserId}");
            }

            _logger.LogInformation($"Received {e.Message} from {e.Client.UserId}");
        }
    }
}