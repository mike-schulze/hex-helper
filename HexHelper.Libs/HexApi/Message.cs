using System;
using HexHelper.Libs.Persistance;
using HexHelper.Libs.Service;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
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
        SaveTalents,
        Ladder
    }

    public enum CollectionAction
    {
        Overwrite,
        Complete,
        Update,
        Unknown
    }

    public interface IMessage
    {
        DateTime Date { get; set; }
        MessageType Type { get; }
        string Summary { get; }
        string User { get; }
        FileInfo SourceFile { get; set; }
    };

    public abstract class MessageBase : IMessage
    {
        public MessageBase( MessageType aType, string aUser, IRepository aRepo, JObject aJson )
        {
            Type = aType;
            User = aUser;

            mRepo = aRepo;

            Setup( aJson );
        }

        private void Setup( JObject aJson )
        {
            if( aJson != null )
            {
                try
                {
                    Parse( aJson );
                    if( mRepo != null )
                    {
                        UpdateRepository();
                    }
                    CreateSummary();
                }
                catch
                {
                    Summary = "Error parsing message.";
                }
            }
        }

        protected virtual void Parse( JObject aJson )
        {
        }

        protected virtual void UpdateRepository()
        {
        }

        protected virtual void CreateSummary()
        {
        }

        public DateTime Date { get; set; } = DateTime.Now;        

        public string Summary { get; protected set; }

        public string User { get; private set; }

        public MessageType Type { get; protected set; } = MessageType.Unknown;

        public FileInfo SourceFile { get; set; }

        protected readonly IRepository mRepo;
    }

    public sealed class GenericMessage : MessageBase
    {
        public GenericMessage( MessageType aType, string aUser ) : base( aType, aUser, aRepo: null, aJson: null )
        {
            Summary = Type.ToString();
        }
    }
}
