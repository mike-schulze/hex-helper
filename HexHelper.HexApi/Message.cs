using System;
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
            CardUpdated
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
            try
            {
                return ( MessageType ) Enum.Parse( typeof( MessageType ), aMessageTypeString );
            }
            catch( InvalidCastException )
            {
                return MessageType.Unknown;
            }
        }
    }
}
