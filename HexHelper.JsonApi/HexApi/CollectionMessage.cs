using System;
using System.Collections.Generic;
using HexHelper.Hex;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public class CollectionMessage : IMessage
    {
        public enum CollectionAction
        {
            Overwrite,
            Complete,
            Unknown
        }

        public CollectionMessage( JObject aJson, bool aLogToFile = true )
        {
            LogToFile = aLogToFile;
            Parse( aJson );
        }

        private void Parse( JObject aJson )
        {
            var theAction = ( string ) aJson["ActionTest"];
            switch( theAction )
            {
                case "Overwrite":
                    Action = CollectionAction.Overwrite;
                    break;
                case "Complete":
                    Action = CollectionAction.Complete;
                    break;
                default:
                    Action = CollectionAction.Unknown;
                    break;
            }

            Complete = ParseArray( ( JArray ) aJson["Complete"] );
            CardsAdded = ParseArray( ( JArray ) aJson["CardsAdded"] );
            CardsRemoved = ParseArray( ( JArray ) aJson["CardsRemoved"] );
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

        public bool LogToFile { get; private set; }
        public MessageType Type { get; private set; } = MessageType.Collection;
        public CollectionAction Action { get; private set; }
    }
}
