namespace Bookstore.TokenManagerService
{
    public static class TokenManager
    {
        public static List<string> blacklistedTokens;
        public static List<string> BlacklistedTokens
        {
            get
            {
                if (blacklistedTokens == null)
                {
                    blacklistedTokens = new List<string>();
                }
                return blacklistedTokens;
            }
        }
        public static void AddToBlacklist(string token)
        {
            blacklistedTokens.Add(token);
        }
    }
}
