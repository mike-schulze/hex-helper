using System;
using System.Collections.Generic;
using HexHelper.Libs.Model;
using HexHelper.Libs.Persistance;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
{
    sealed public class SaveTalentsMessage : MessageBase
    {
        public SaveTalentsMessage( JObject aJson, string aUser, IRepository aRepo ) : base( EMessageType.SaveTalents, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {            
            Race =  ( ERace ) Enum.Parse( typeof( ERace) , ( string ) aJson["Race"] );
            Class =( EClass ) Enum.Parse( typeof( EClass ), ( string ) aJson["Class"] );
            Champion = ( string ) aJson["Champion"];

            Talents = new List<string>();
            var theTalent = ( JArray ) aJson["Picks"];
            if( theTalent != null )
            {
                foreach( var theItem in theTalent )
                {
                    Talents.Add( ( string ) theItem["Name"] );
                }
            }
        }

        protected override void CreateSummary()
        {
            Summary = String.Format( "{0} {1} talents saved for '{2}'.", Race, Class, Champion );
        }

        public ERace Race { get; private set; }
        public EClass Class { get; private set; }
        public string Champion { get; private set; }
        public IList<string> Talents { get; private set; }
    }
}
