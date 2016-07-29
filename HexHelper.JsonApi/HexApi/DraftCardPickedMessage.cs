using System;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class DraftCardPickedMessage : MessageBase
    {
        public DraftCardPickedMessage( JObject aJson, string aUser, IRepository aRepo ) : base( MessageType.DraftCardPicked, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            var theGuidString = (string) aJson["Card"]["Guid"]["m_Guid"];
            CardPicked = mRepo?.GetItem( theGuidString );
        }

        protected override void CreateSummary()
        {
            if( CardPicked == null )
            {
                Summary = "Unknown Card Picked";
            }
            else
            {
                Summary = String.Format( "Picked '{0}'.", CardPicked.Name );
            }
        }

        public Info CardPicked { get; private set; }
    }
}
