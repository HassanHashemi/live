using System;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public class RedisBackplane : IBackplane
    {
        public event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;

        public Task Publish(BackplaneMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
