using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public static class Parser
    {
        public static IMessage ParseMessage( string aMessageString )
        {
            MessageType theType = MessageType.Unknown;
            string theUser = null;
            try
            {
                var theJson = JObject.Parse( aMessageString );
                theType = TypeFromString( ( string ) theJson["Message"] );
                theUser = ( string ) theJson["User"];

                switch( theType )
                {
                    case MessageType.Collection:
                        return new CollectionMessage( theJson, theUser );
                    case MessageType.DraftPack:
                        return new DraftPackMessage( theJson, theUser );
                    case MessageType.DraftCardPicked:
                        return new DraftCardPickedMessage( theJson, theUser );
                    default:
                        break;
                }
            }
            catch
            {

            }

            return new GenericMessage( theType, theUser );
        }

        private static MessageType TypeFromString( string aMessageTypeString )
        {
            MessageType theType;
            if( Enum.TryParse( aMessageTypeString, out theType ) )
            {
                return theType;
            }

            Debug.WriteLine( String.Format( "Unknown message type: {0}", aMessageTypeString ) );
            return MessageType.Unknown;
        }
    }
}
