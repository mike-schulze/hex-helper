using System;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class LadderMessage : MessageBase
    {
        public LadderMessage( JObject aJson, string aUser, IRepository aRepo ) : base( MessageType.Ladder, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            Tier = ( int ) aJson["Tier"];
            CosmicRank = ( int ) aJson["CosmicRank"];
            Division = (DivisionType) Enum.Parse( typeof( DivisionType ), (string) aJson["Division"] );
            Format = ( FormatType ) Enum.Parse( typeof( FormatType ), ( string ) aJson["Type"] );
        }

        protected override void CreateSummary()
        {
            if( CosmicRank == 0 )
            {
                Summary = String.Format( "{0} Ladder ({1} {2})", Format.ToString(), Division.ToString(), Tier );
            }
            else
            {
                Summary = String.Format( "Constructed Ladder (Cosmic Rank {0})", CosmicRank );
            }            
        }

        public int Tier { get; private set; }
        public DivisionType Division { get; private set; }
        public FormatType Format { get; private set; }
        public int CosmicRank { get; private set; }
    }
}
