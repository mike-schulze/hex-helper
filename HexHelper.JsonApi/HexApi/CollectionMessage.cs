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

        private IEnumerable<ObjectCount> ParseArray( JArray aObjects )
        {
            var theObjects = new List<ObjectCount>();
            if( aObjects != null )
            {
                foreach( var theObject in aObjects )
                {
                    theObjects.Add( new ObjectCount()
                    {
                        Id = new Guid( ( string ) theObject["Guid"]["m_Guid"] ),
                        Count = (int) theObject["Count"]
                    } );
                }
            }
            return theObjects;
        }

        public IEnumerable<ObjectCount> Complete { get; private set; }
        public IEnumerable<ObjectCount> CardsAdded { get; private set; }
        public IEnumerable<ObjectCount> CardsRemoved { get; private set; }

        public bool LogToFile { get; private set; }
        public MessageType Type { get; private set; } = MessageType.Collection;
        public CollectionAction Action { get; private set; }
    }
}
