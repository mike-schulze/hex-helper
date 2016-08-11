using System;
using System.Collections.Generic;
using HexHelper.Libs.Model;
using HexHelper.Libs.Persistance;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
{
    public sealed class InventoryMessage : MessageBase
    {
        public InventoryMessage( JObject aJson, string aUser, IRepository aRepo ) : base( EMessageType.Inventory, aUser, aRepo, aJson )
        {
        }

        protected override void Parse( JObject aJson )
        {
            var theActionString = ( string ) aJson["Action"];
            ECollectionAction theActionType;
            if( Enum.TryParse( theActionString, out theActionType ) )
            {
                Action = theActionType;
            }
            else
            {
                Action = ECollectionAction.Unknown;
            }

            Complete = ParseArray( ( JArray ) aJson["Complete"] );
            ItemsAdded = ParseArray( ( JArray ) aJson["ItemsAdded"] );
            ItemsRemoved = ParseArray( ( JArray ) aJson["ItemsRemoved"] );
        }

        protected override void UpdateRepository()
        {
            mRepo.UpdateUserInventory( User, Complete );
            if( ItemsAdded != null )
            {
                foreach( var theItem in ItemsAdded )
                {
                    mRepo.UpdateInventoryQuantities( User, theItem.Key, theItem.Value.QuantityOwned );
                }
            }
            if( ItemsRemoved != null )
            {
                foreach( var theItem in ItemsRemoved )
                {
                    mRepo.UpdateInventoryQuantities( User, theItem.Key, theItem.Value.QuantityOwned * -1 );
                }
            }
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

        protected override void CreateSummary()
        {
            string theSummary = String.Format( "[{0}] ", Action.ToString() );
            if( Complete.Count != 0 )
            {
                theSummary += String.Format( "Complete inventory. {0} items. ", Complete.Count );
            }
            if( ItemsAdded.Count != 0 )
            {
                theSummary += String.Format( "{0} inventory item(s) added. ", ItemsAdded.Count );
            }
            if( ItemsRemoved.Count != 0 )
            {
                theSummary += String.Format( "{0} inventory item(s) removed. ", ItemsRemoved.Count );
            }
            Summary = theSummary;
        }

        public IDictionary<Guid, CollectionInfo> Complete { get; private set; }
        public IDictionary<Guid, CollectionInfo> ItemsAdded { get; private set; }
        public IDictionary<Guid, CollectionInfo> ItemsRemoved { get; private set; }

        public ECollectionAction Action { get; private set; }
    }
}
