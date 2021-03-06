﻿using System;
using HexHelper.Libs.Model;
using HexHelper.Libs.Persistance;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
{
    public sealed class LadderMessage : MessageBase
    {
        public LadderMessage( JObject aJson, string aUser, IRepository aRepo ) : base( EMessageType.Ladder, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            Tier = ( int ) aJson["Tier"];
            CosmicRank = ( int ) aJson["CosmicRank"];
            Division = ( EDivision ) Enum.Parse( typeof( EDivision ), (string) aJson["Division"] );
            Format = ( EFormat ) Enum.Parse( typeof( EFormat ), ( string ) aJson["Type"] );
        }

        protected override void CreateSummary()
        {
            if( CosmicRank == 0 )
            {
                Summary = String.Format( "{0} Ladder ({1} {2})", Format.ToString(), Division.ToString(), Tier );
            }
            else
            {
                Summary = String.Format( "{0} Ladder (Cosmic Rank {1})", Format.ToString(), CosmicRank );
            }            
        }

        public int Tier { get; private set; }
        public EDivision Division { get; private set; }
        public EFormat Format { get; private set; }
        public int CosmicRank { get; private set; }
    }
}
