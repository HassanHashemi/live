namespace Live.Hub
{
    public static class Extensions
    {
        public static bool IsHeartbeat(this byte[] source)
        {
            if (source.Length != 1)
            {
                return false;
            }

            return source[0] == 0;
        }
    }
}
