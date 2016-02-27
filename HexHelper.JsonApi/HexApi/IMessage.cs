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
        MessageType Type { get; }
        bool LogToFile { get;  }
    };

    public class GenericMessage : IMessage
    {
        public GenericMessage( MessageType aType, bool aLogToFile = false )
        {
            Type = aType;
            LogToFile = aLogToFile;
        }

        public bool LogToFile { get; private set; } = false;

        public MessageType Type { get; private set; } = MessageType.Collection;
    }
}
