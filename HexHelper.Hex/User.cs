using System.Collections.Generic;

namespace HexHelper.Hex
{
    public sealed class User
    {
        public User( string aUserName, string aTcgBrowserSyncCode = null )
        {
            UserName = aUserName;
        }

        public string UserName { get; set; }

        /// <summary>
        /// Which Hex API sites does user want to forward data to.
        /// First: enum value as string, Second: API key (or emptry if no key needed)
        /// TODO: use enum instead of string,
        /// </summary>
        public Dictionary<string, string> ApiSites { get; } = new Dictionary<string, string>();
    }
}
