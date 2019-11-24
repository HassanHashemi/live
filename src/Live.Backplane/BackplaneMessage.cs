using System;

namespace Live.Backplane
{
    public class BackplaneMessage
    {
        public BackplaneMessage()
        {
        }

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

            this.UserId = userId;
            this.MerchantId = merchantId;
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public string MerchantId { get; set; }
        public string UserId { get; set; }
        public object Message { get; set; }
    }
}
