using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public class RedisBackplaine : IBackplaine
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;
        private ISubscriber _subscriber;
        public const string CHANNEL_NAME = "live";

        public RedisBackplaine(IConnectionMultiplexer connectionMultiplexer)
        {
            this._connectionMultiplexer = connectionMultiplexer;
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

        public async Task Init()
        {
            _subscriber = this._connectionMultiplexer.GetSubscriber();

            var s = await _subscriber.SubscribeAsync(CHANNEL_NAME);
            s.OnMessage(m =>
            {
                var message = JsonSerializer.Deserialize<BackplaneMessage>(m.Message);

                MessageReceived?.Invoke(this, new BackplaneMessageReceivedArgs
                {
                    Message = message
                });
            });
        }
    }
}
