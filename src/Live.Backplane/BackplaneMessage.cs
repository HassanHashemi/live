using System;

namespace Live.Backplane
{
    public class BackplaneMessage
    {
        public BackplaneMessage(string clientId, object message)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.ClientId = clientId;
            this.Message = message;
        }

        public string ClientId { get; private set; }
        public object Message { get; private set; }
    }
}
