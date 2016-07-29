using System;
using System.Collections.Generic;
using HexHelper.Hex;
using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public sealed class CollectionMessage : MessageBase
    {
        public enum CollectionAction
        {
            Overwrite,
            Complete,
            Update,
            Unknown
        }

        public CollectionMessage( JObject aJson, string aUser, IRepository aRepo ) : base( MessageType.Collection, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            var theActionString = ( string ) aJson["Action"];
            CollectionAction theActionType;
            if( Enum.TryParse( theActionString , out theActionType ) )
            {
                Action = theActionType;
            }
            else
            {
                Action = CollectionAction.Unknown;
            }

            Complete = ParseArray( ( JArray ) aJson["Complete"] );
            CardsAdded = ParseArray( ( JArray ) aJson["CardsAdded"] );
            CardsRemoved = ParseArray( ( JArray ) aJson["CardsRemoved"] );
        }

        protected override void UpdateRepository()
        {
            mRepo.UpdateInventory( User, Complete );
            if( CardsAdded != null )
            {
                foreach( var theItem in CardsAdded )
                {
                    mRepo.UpdateCopiesOwned( User, theItem.Key, theItem.Value.CopiesOwned );
                }
            }
            if( CardsRemoved != null )
            {
                foreach( var theItem in CardsRemoved )
                {
                    mRepo.UpdateCopiesOwned( User, theItem.Key, theItem.Value.CopiesOwned * -1 );
                }
            }
        }

        protected override void CreateSummary()
        {
            string theSummary = String.Format( "[{0}] ", Action.ToString() );
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
                        theObjects.Add( theId, new CollectionInfo() { CopiesOwned = theCount } );
                    }
                    else
                    {
                        theObjects[theId].CopiesOwned += theCount;
                    }
                }
            }
            return theObjects;
        }

        public IDictionary<Guid, CollectionInfo> Complete { get; private set; }
        public IDictionary<Guid, CollectionInfo> CardsAdded { get; private set; }
        public IDictionary<Guid, CollectionInfo> CardsRemoved { get; private set; }

        public CollectionAction Action { get; private set; }
    }
}
