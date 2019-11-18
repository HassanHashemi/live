using System;
using System.Threading;
using System.Threading.Tasks;

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

            Complition = new TaskCompletionSource<object>();

            UserId = userId;
            MerchantId = merchantId;
            CancellationToken = cancellationToken;
        }

        public string UserId { get; }
        public TaskCompletionSource<object> Complition { get; } = new TaskCompletionSource<object>();
        public string MerchantId { get; }
        public CancellationToken CancellationToken { get; }
    }
}
