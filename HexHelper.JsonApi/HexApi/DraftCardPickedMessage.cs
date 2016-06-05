using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class DraftCardPickedMessage : MessageBase
    {
        public DraftCardPickedMessage( JObject aJson, string aUser ) : base( MessageType.DraftCardPicked, aUser, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
        }
    }
}
