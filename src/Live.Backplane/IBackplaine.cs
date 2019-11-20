using System;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public interface IBackplaine
    {
        Task Connect();
        Task Publish(BackplaneMessage message, CancellationToken cancellationToken);
        event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;
    }
}
