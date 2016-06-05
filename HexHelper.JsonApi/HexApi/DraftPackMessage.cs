using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class DraftPackMessage : MessageBase
    {
        public DraftPackMessage( JObject aJson, string aUser ) : base( MessageType.DraftPack, aUser, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
        }
    }
}
