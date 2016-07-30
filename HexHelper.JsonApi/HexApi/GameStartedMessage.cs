using System;
using System.Collections.Generic;
using System.Linq;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class GameStartedMessage : MessageBase
    {
        public GameStartedMessage( JObject aJson, string aUser, IRepository aRepo ) : base( MessageType.GameStarted, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            var thePlayersJson = aJson["Players"];
            foreach( var theChampion in thePlayersJson )
            {
                if( PlayerChampion == null )
                {
                    PlayerChampion = ( string ) theChampion;
                }
                else
                {
                    OpponentChampion = ( string ) theChampion;
                }
            }
        }

        protected override void CreateSummary()
        {
            if( PlayerChampion == null || OpponentChampion == null )
            {
                Summary = "New Game Started.";
            }
            else
            {
                Summary = String.Format( "Game started: {0} vs. {1}.", PlayerChampion, OpponentChampion );
            }
        }

        public string PlayerChampion { get; private set; }
        public string OpponentChampion { get; private set; }
    }
}
