using System;
using System.Collections.Generic;
using HexHelper.Libs.Model;
using HexHelper.Libs.Persistance;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
{
    public sealed class CollectionMessage : MessageBase
    {
        public CollectionMessage( JObject aJson, string aUser, IRepository aRepo ) : base( EMessageType.Collection, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            var theActionString = ( string ) aJson["Action"];
            ECollectionAction theActionType;
            if( Enum.TryParse( theActionString , out theActionType ) )
            {
                Action = theActionType;
            }
            else
            {
                Action = ECollectionAction.Unknown;
            }

            Complete = ParseArray( ( JArray ) aJson["Complete"] );
            CardsAdded = ParseArray( ( JArray ) aJson["CardsAdded"] );
            CardsRemoved = ParseArray( ( JArray ) aJson["CardsRemoved"] );
        }

        protected override void UpdateRepository()
        {
            mRepo.UpdateUserCardCollection( User, Complete );
            if( CardsAdded != null )
            {
                foreach( var theItem in CardsAdded )
                {
                    mRepo.UpdateCardQuantities( User, theItem.Key, theItem.Value.QuantityOwned );
                }
            }
            if( CardsRemoved != null )
            {
                foreach( var theItem in CardsRemoved )
                {
                    mRepo.UpdateCardQuantities( User, theItem.Key, theItem.Value.QuantityOwned * -1 );
                }
            }
        }

        protected override void CreateSummary()
        {
            string theSummary;

            int theAdditionsAndRemovals = CardsAdded.Count + CardsRemoved.Count;
            if( Complete.Count == 0 && theAdditionsAndRemovals >= 0 && theAdditionsAndRemovals <= 3 )
            {
                var theChanges = new List<string>();
                foreach( var theCard in CardsAdded )
                {
                    var theItem = mRepo.GetItem( theCard.Key.ToString() );
                    if( theItem != null )
                    {
                        theChanges.Add( String.Format( "'{0}' added", theItem.Name ) );
                    }                    
                }

                foreach( var theCard in CardsRemoved )
                {
                    var theItem = mRepo.GetItem( theCard.Key.ToString() );
                    if( theItem != null )
                    {
                        theChanges.Add( String.Format( "'{0}' removed", theItem.Name ) );
                    }
                }

                theSummary = String.Join( ", ", theChanges );
            }
            else
            {
                theSummary = String.Format( "[{0}] ", Action.ToString() );

                if( Complete.Count != 0 )
                {
                    theSummary += String.Format( "Complete collection. {0} items. ", Complete.Count );
                }
                if( CardsAdded.Count != 0 )
                {
                    theSummary += String.Format( "{0} item(s) added. ", CardsAdded.Count );
                }
                if( CardsRemoved.Count != 0 )
                {
                    theSummary += String.Format( "{0} item(s) removed. ", CardsRemoved.Count );
                }
            }
            Summary = theSummary;
        }

        private IDictionary<Guid, CollectionInfo> ParseArray( JArray aObjects )
        {
            var theObjects = new Dictionary<Guid, CollectionInfo>();
            if( aObjects != null )
            {
                foreach( var theObject in aObjects )
                {
                    Guid theId = new Guid( ( string ) theObject["Guid"]["m_Guid"] );
                    int theCount = ( int ) theObject["Count"];

                    if( !theObjects.ContainsKey( theId ) )
                    {
                        theObjects.Add( theId, new CollectionInfo() { QuantityOwned = theCount } );
                    }
                    else
                    {
                        theObjects[theId].QuantityOwned += theCount;
                    }
                }
            }
            return theObjects;
        }

        public IDictionary<Guid, CollectionInfo> Complete { get; private set; }
        public IDictionary<Guid, CollectionInfo> CardsAdded { get; private set; }
        public IDictionary<Guid, CollectionInfo> CardsRemoved { get; private set; }

        public ECollectionAction Action { get; private set; }
    }
}
