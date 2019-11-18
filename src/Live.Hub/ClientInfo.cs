using System;
using System.Threading;

namespace Live.Hub
{
    public class ClientInfo
    {
        public ClientInfo(string merchantId, string userId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(merchantId))
            {
                throw new ArgumentNullException(nameof(merchantId));
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            UserId = userId;
            MerchantId = merchantId;
            CancellationToken = cancellationToken;
        }

        public string UserId { get; }
        public string MerchantId { get; }
        public CancellationToken CancellationToken { get; }
    }
}
