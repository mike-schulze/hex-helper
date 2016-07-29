using System;
using System.Collections.Generic;
using System.Linq;
using HexHelper.Hex;
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
            var theCards = new List<Info>();
            var theCardsJson = aJson["Cards"];
            foreach( var theCard in theCardsJson )
            {
                string theGuidString = ( string ) theCard["Guid"]["m_Guid"];
                var theItem = mRepo?.GetItem( theGuidString );
                if( theItem != null )
                {
                    theCards.Add( theItem );
                }
            }
            Cards = theCards;
        }

        protected override void CreateSummary()
        {
            if( Cards == null || !Cards.Any() )
            {
                Summary = "New Draft Pack";
            }
            else
            {
                Summary = String.Format( "New Draft Pack (#{0})", 18 - Cards.Count() );
            }
        }

        public IEnumerable<Info> Cards { get; private set; }
    }
}
