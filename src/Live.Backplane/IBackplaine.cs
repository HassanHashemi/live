using System;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public interface IBackplaine
    {
        Task Publish(BackplaneMessage message, CancellationToken cancellationToken = default);
        Task Init();

        event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;
    }
}
