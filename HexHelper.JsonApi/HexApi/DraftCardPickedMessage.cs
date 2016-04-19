using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class DraftCardPickedMessage : MessageBase
    {
        public DraftCardPickedMessage( JObject aJson, string aUser ) : base( MessageType.DraftCardPicked, aUser, true, aJson )
        {
            SupportsHexTcgBrowser = true;
        }

        protected override void Parse( JObject aJson )
        {
        }
    }
}
