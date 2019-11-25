using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public class RedisBackplaine : IBackplaine
    {
        public const string CHANNEL_NAME = "live";

        private readonly string _redisAddress;
        private readonly ILogger<RedisBackplaine> _logger;

        private ISubscriber _subscriber;
        private IConnectionMultiplexer _connectionMultiplexer;

        public RedisBackplaine(IOptions<RedisConfig> config, ILogger<RedisBackplaine> logger)
        {
            if (string.IsNullOrEmpty(config.Value?.Address))
            {
                throw new ArgumentNullException("Invalid redis config");
            }

            _redisAddress = config.Value.Address;
            _logger = logger;
        }

        public event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;
        private bool Initialized => _subscriber != null;

        public Task Publish(BackplaneMessage message, CancellationToken cancellationToken = default)
        {
            if (!_connectionMultiplexer.IsConnected)
            {
                throw new InvalidOperationException("Connection Multiplexer must be connected");
            }

            var db = _connectionMultiplexer.GetDatabase();
            var json = JsonSerializer.Serialize(message);

            return db.PublishAsync(CHANNEL_NAME, json, CommandFlags.FireAndForget);
        }

        public async Task Init()
        {
            if (Initialized)
            {
                return;
            }

            _connectionMultiplexer = await ConnectionMultiplexer.ConnectAsync(_redisAddress);
            _subscriber = _connectionMultiplexer.GetSubscriber();

            await _subscriber.SubscribeAsync(CHANNEL_NAME, HandleMessage);
        }

        private void HandleMessage(RedisChannel channel, RedisValue redisMessage)
        {
            if (channel != CHANNEL_NAME)
            {
                _logger.LogWarning("Invalid channel name received");
                return;
            }

            var message = JsonSerializer.Deserialize<BackplaneMessage>(redisMessage);

            if (message == null)
            {
                _logger.LogError($"Error deserializing {redisMessage.ToString()}");
                return;
            }

            MessageReceived?.Invoke(this, new BackplaneMessageReceivedArgs
            {
                Message = message
            });
        }
    }
}
