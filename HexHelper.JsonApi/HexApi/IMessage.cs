using System;

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
    };

    public class GenericMessage : IMessage
    {
        public GenericMessage( MessageType aType, bool aLogToFile = false )
        {
            Type = aType;
            LogToFile = aLogToFile;
        }

        public DateTime Date { get; private set; } = DateTime.Now;

        public bool LogToFile { get; private set; } = false;

        public MessageType Type { get; private set; } = MessageType.Collection;

        public string Summary {
            get
            {
                return Type.ToString();
            }
        }
    }
}
