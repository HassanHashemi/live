using System;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public class RedisBackplaine : IBackplaine
    {
        public event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;

        public Task Connect()
        {
            return Task.CompletedTask;
        }

        public Task Publish(BackplaneMessage message, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
