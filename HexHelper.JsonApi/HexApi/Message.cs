using System;
using Newtonsoft.Json.Linq;

namespace HexHelper.JsonApi.HexApi
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

    public interface IMessage
    {
        DateTime Date { get; }
        MessageType Type { get; }
        bool LogToFile { get; }
        string Summary { get; }
        string User { get; }
        bool SupportsHexTcgBrowser { get; }
    };

    public abstract class MessageBase : IMessage
    {
        public MessageBase( MessageType aType, string aUser, bool aLogToFile = false, JObject aJson = null )
        {
            Type = aType;
            User = aUser;
            LogToFile = aLogToFile;

            if( aJson != null )
            {
                Parse( aJson );
            }            
        }

        protected virtual void Parse( JObject aJson )
        {

        }

        public DateTime Date { get; } = DateTime.Now;        

        public string Summary { get; protected set; }

        public string User { get; private set; }

        public MessageType Type { get; protected set; } = MessageType.Unknown;

        public bool LogToFile { get; protected set; }

        public bool SupportsHexTcgBrowser { get; protected set; } = false;
    }

    public sealed class GenericMessage : MessageBase
    {
        public GenericMessage( MessageType aType, string aUser, bool aLogToFile = false ) : base( aType, aUser, aLogToFile )
        {
            Summary = Type.ToString();
        }
    }
}
