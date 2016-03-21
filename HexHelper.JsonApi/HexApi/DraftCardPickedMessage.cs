using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class DraftCardPickedMessage : MessageBase
    {
        public DraftCardPickedMessage( JObject aJson ) : base( MessageType.DraftCardPicked, true, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
        }
    }
}
