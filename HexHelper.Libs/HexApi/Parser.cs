using System;
using System.Diagnostics;
using HexHelper.Libs.Persistance;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
{
    public static class Parser
    {
        public static IMessage ParseMessage( string aMessageString, IRepository aRepo )
        {
            EMessageType theType = EMessageType.Unknown;
            string theUser = null;
            try
            {
                var theJson = JObject.Parse( aMessageString );
                theType = TypeFromString( ( string ) theJson["Message"] );
                theUser = ( string ) theJson["User"];

                switch( theType )
                {
                    case EMessageType.Collection:
                        return new CollectionMessage( theJson, theUser, aRepo );
                    case EMessageType.DraftCardPicked:
                        return new DraftCardPickedMessage( theJson, theUser, aRepo );
                    case EMessageType.DraftPack:
                        return new DraftPackMessage( theJson, theUser, aRepo );
                    case EMessageType.GameEnded:
                        return new GameEndedMessage( theJson, theUser, null );
                    case EMessageType.GameStarted:
                        return new GameStartedMessage( theJson, theUser, null );
                    case EMessageType.Inventory:
                        return new InventoryMessage( theJson, theUser, aRepo );
                    case EMessageType.Ladder:
                        return new LadderMessage( theJson, theUser, aRepo: null );
                    case EMessageType.SaveDeck:
                        return new SaveDeckMessage( theJson, theUser, aRepo: null );
                    case EMessageType.SaveTalents:
                        return new SaveTalentsMessage( theJson, theUser, aRepo: null );
                    case EMessageType.Tournament:
                        return new TournamentMessage( theJson, theUser, null );
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

        private static EMessageType TypeFromString( string aMessageTypeString )
        {
            EMessageType theType;
            if( Enum.TryParse( aMessageTypeString, out theType ) )
            {
                return theType;
            }

            Debug.WriteLine( String.Format( "Unknown message type: {0}", aMessageTypeString ) );
            return EMessageType.Unknown;
        }
    }
}
