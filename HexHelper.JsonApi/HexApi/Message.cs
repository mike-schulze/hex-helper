using System;
using HexHelper.Hex.Interface;
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
        PlayerUpdated,
        Tournament,
        SaveTalents
    }

    public interface IMessage
    {
        DateTime Date { get; }
        MessageType Type { get; }
        string Summary { get; }
        string User { get; }
        bool SupportsHexTcgBrowser { get; }
        FileInfo SourceFile { get; set; }
    };

    public abstract class MessageBase : IMessage
    {
        public MessageBase( MessageType aType, string aUser, JObject aJson = null )
        {
            Type = aType;
            User = aUser;

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

        public bool SupportsHexTcgBrowser { get; protected set; } = false;

        public FileInfo SourceFile { get; set; }
    }

    public sealed class GenericMessage : MessageBase
    {
        public GenericMessage( MessageType aType, string aUser ) : base( aType, aUser )
        {
            Summary = Type.ToString();
        }
    }
}
