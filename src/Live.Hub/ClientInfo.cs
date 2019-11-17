using System;
using System.Threading;
using System.Threading.Tasks;

namespace Live.Hub
{
    public class ClientInfo
    {
        public ClientInfo(string merchantId, string userId, TaskCompletionSource<object> tcs, CancellationToken cancellationToken = default)
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
            Complition = tcs;
            MerchantId = merchantId;
            CancellationToken = cancellationToken;
        }

        public string UserId { get; }
        public TaskCompletionSource<object> Complition { get; }
        public string MerchantId { get; }
        public CancellationToken CancellationToken { get; }
    }
}
