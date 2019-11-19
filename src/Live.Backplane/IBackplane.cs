using System;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public interface IBackplane
    {
        Task Publish(BackplaneMessage message) => Publish(message, default);
        Task Publish(BackplaneMessage message, CancellationToken cancellationToken);
        event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;
    }
}
