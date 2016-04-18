namespace HexHelper.Hex
{
    public sealed class User
    {
        public User( string aUserName, string aTcgBrowserSyncCode = null )
        {
            UserName = aUserName;
            TcgBrowserSyncCode = aTcgBrowserSyncCode;
        }

        public string UserName { get; set; }
        public string TcgBrowserSyncCode { get; set; }
    }
}
