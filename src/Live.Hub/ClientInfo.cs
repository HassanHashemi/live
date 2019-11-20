using System;

namespace Live.Hub
{
    public class ClientInfo
    {
        public ClientInfo(string merchantId, string userId)
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
        }

        public string UserId { get; }
        public string MerchantId { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is ClientInfo other))
            {
                return false;
            }

            return this == other;
        }

        public override int GetHashCode()
        {
            return string.Concat(MerchantId, UserId).GetHashCode();
        }

        public static bool operator ==(ClientInfo left, ClientInfo right)
        {
            if (left is null && right is null)
            {
                return true;
            }

            if (left is null || right is null)
            {
                return false;
            }

            return left.MerchantId == right.MerchantId && left.UserId == right.UserId;
        }

        public static bool operator !=(ClientInfo left, ClientInfo right)
        {
            if (left is null && right is null)
            {
                return false;
            }

            if (left is null || right is null)
            {
                return true;
            }

            return left.MerchantId != right.MerchantId || left.UserId != right.UserId;
        }
    }
}
