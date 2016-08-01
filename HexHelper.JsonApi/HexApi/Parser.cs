using System;
using System.Diagnostics;
using HexHelper.Hex.Interface;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
{
    public static class Parser
    {
        public static IMessage ParseMessage( string aMessageString, IRepository aRepo )
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
                        return new CollectionMessage( theJson, theUser, aRepo );
                    case MessageType.DraftCardPicked:
                        return new DraftCardPickedMessage( theJson, theUser, aRepo );
                    case MessageType.DraftPack:
                        return new DraftPackMessage( theJson, theUser, aRepo );
                    case MessageType.GameEnded:
                        return new GameEndedMessage( theJson, theUser, null );
                    case MessageType.GameStarted:
                        return new GameStartedMessage( theJson, theUser, null );
                    case MessageType.Inventory:
                        return new InventoryMessage( theJson, theUser, aRepo );
                    case MessageType.Ladder:
                        return new LadderMessage( theJson, theUser, aRepo: null );
                    default:
                        break;
                }
            }
            catch
            {
                // Fall back to generic message
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
