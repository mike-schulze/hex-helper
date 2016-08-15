using System;
using System.Collections.Generic;
using HexHelper.Libs.Persistance;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
{
    public sealed class SaveDeckMessage : MessageBase
    {
        public SaveDeckMessage( JObject aJson, string aUser, IRepository aRepo ) : base( EMessageType.SaveDeck, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            Name = ( string ) aJson["Name"];
            Champion = ( string ) aJson["Champion"];

            try
            {
                IsTournamentDeck = ( bool ) aJson["TournamentDeck"];
            }
            catch( ArgumentNullException )
            {
                IsTournamentDeck = false;
            }

            Equipment = new List<Guid>();            
            var theEquipment = ( JArray ) aJson["Equipment"];
            if( theEquipment != null )
            {
                foreach( var theItem in theEquipment )
                {
                    Equipment.Add( new Guid( ( string ) theItem["m_Guid"] ) );
                }
            }

            Cards = new List<Guid>();
            var theDeck = ( JArray ) aJson["Deck"];
            if( theDeck != null )
            {
                foreach( var theCard in theDeck )
                {
                    Cards.Add( new Guid( ( string ) theCard["Guid"]["m_Guid"] ) );
                }
            }
        }

        protected override void CreateSummary()
        {
            if( IsTournamentDeck )
            {
                Summary = String.Format( "Limited deck ('{0}') saved.", Champion );
            }
            else
            {
                Summary = String.Format( "Deck '{0}' ('{1}') saved.", Name, Champion );
            }        
        }

        public string Name { get; private set; }
        public string Champion { get; private set; }
        public IList<Guid> Cards { get; private set; }
        public IList<Guid> Equipment { get; private set; }
        public bool IsTournamentDeck { get; private set; }
    }
}
