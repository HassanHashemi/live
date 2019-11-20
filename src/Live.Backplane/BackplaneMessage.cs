using System;

namespace Live.Backplane
{
    public class BackplaneMessage
    {
        public BackplaneMessage(string userId, string merchantId, object message)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            if (string.IsNullOrEmpty(merchantId))
            {
                throw new ArgumentException(nameof(merchantId));
            }

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.UserId = userId;
            this.Message = message;
        }

        public string MerchantId { get; set; }
        public string UserId { get; private set; }
        public object Message { get; private set; }
    }
}
