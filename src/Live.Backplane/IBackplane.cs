using System;
using System.Threading.Tasks;

namespace Live.Backplane
{
    public interface IBackplane
    {
        Task Publish(BackplaneMessage message);
        event EventHandler<BackplaneMessageReceivedArgs> MessageReceived;
    }
}
