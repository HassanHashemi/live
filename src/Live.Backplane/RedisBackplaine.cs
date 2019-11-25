using Microsoft.Extensions.Logging;
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

        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private readonly ILogger<RedisBackplaine> _logger;
        private ISubscriber _subscriber;

        public RedisBackplaine(IConnectionMultiplexer connectionMultiplexer, ILogger<RedisBackplaine> logger)
        {
            if (!_connectionMultiplexer.IsConnected)
            {
                throw new ArgumentException("ConnectionMultiplexer must be connected before using");
            }

            _connectionMultiplexer = connectionMultiplexer;
            _logger = logger;
        }

        public event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;

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

        public Task Init()
        {
            _subscriber = _connectionMultiplexer.GetSubscriber();

            return _subscriber.SubscribeAsync(CHANNEL_NAME, HandleMessage);
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
