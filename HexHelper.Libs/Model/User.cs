using System.Collections.Generic;
using HexHelper.Libs.HexApi;
using HexHelper.Libs.WebApiForward;

namespace HexHelper.Libs.Model
{
    public sealed class User
    {
        public sealed class UserApiForward
        {
            public EApiSites Type { get; set; }
            public string ApiKey { get; set; }
            public List<EMessageType> Messages { get; set; }
            public bool ForwardToThis { get; set; }
        }

        public User( string aUserName )
        {
            UserName = aUserName;
        }

        public string UserName { get; set; }

        public List<UserApiForward> Forwards { get; } = new List<UserApiForward>();
    }
}
