using System;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace HexHelper.HexApi
{
    public class Message
    {        
        public enum MessageType
        {
            Unknown,
            Talents,
            Inventory,
            Collection,
            Login,
            Logout,
            SaveDeck,
            DraftPack,
            DraftCardPicked,
            GameStarted,
            GameEnded,
            PlayedUpdated,
            CardUpdated,
            PlayerUpdated
        }

        public MessageType Type { get; private set; }

        public static Message ParseMessage( string aMessageString )
        {
            MessageType theType = MessageType.Unknown;
            try
            {
                var theJson = JObject.Parse( aMessageString );
                theType = Message.TypeFromString( ( string ) theJson["Message"] );
            }
            catch
            {

            }            

            return new Message() { Type = theType };
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
