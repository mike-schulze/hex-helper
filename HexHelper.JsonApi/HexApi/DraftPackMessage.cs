using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class DraftPackMessage : MessageBase
    {
        public DraftPackMessage( JObject aJson, string aUser, IRepository aRepo ) : base( MessageType.DraftPack, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
        }

        protected override void CreateSummary()
        {
            Summary = "New Draft Pack";
        }
    }
}
