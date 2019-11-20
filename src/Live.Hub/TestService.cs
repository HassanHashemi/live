using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class TestService : IHostedService
    {
        private readonly SocketServer _socketServer;
        private readonly ILogger<TestService> _logger;
        private Timer _pushTimer;

        public TestService(SocketServer socketServer, ILogger<TestService> logger)
        {
            _socketServer = socketServer;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _socketServer.ClientConnected += SocketServer_ClientConnected;
            _socketServer.JsonReceived += SocketServer_JsonReceived;
            _socketServer.ClientDisconnected += SocketServer_ClientDisconnected;
            _socketServer.ClientHeartBeat += SocketServer_ClientHeartBeat;
            _pushTimer = new Timer(async o =>
            {
                await _socketServer.SendJson("38", new { msg = "Hellow??" });
            }, null, 0, 3000);

            return Task.CompletedTask;
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

        private void SocketServer_JsonReceived(object sender, MessageReceivedArgs<string> e)
        {
            _logger.LogInformation($"Received {e.Message} from {e.Client.UserId}");
        }
    }
}
