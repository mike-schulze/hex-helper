using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class DraftPackMessage : MessageBase
    {
        public DraftPackMessage( JObject aJson ) : base( MessageType.DraftPack, true, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
        }
    }
}
