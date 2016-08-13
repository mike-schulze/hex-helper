using System;
using HexHelper.Libs.Persistance;
using HexHelper.Libs.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace HexHelper.Libs.HexApi
{

    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum EMessageType
    {
        Unknown,
        Inventory,
        Collection,
        Login,
        Logout,
        SaveDeck,
        DraftPack,
        DraftCardPicked,
        GameStarted,
        GameEnded,
        CardUpdated,
        PlayerUpdated,
        Tournament,
        SaveTalents,
        Ladder
    }

    [JsonConverter( typeof( StringEnumConverter ) )]
    public enum ECollectionAction
    {
        Overwrite,
        Complete,
        Update,
        Unknown
    }

    public interface IMessage
    {
        DateTime Date { get; set; }
        EMessageType Type { get; }
        string Summary { get; }
        string User { get; }
        FileInfo SourceFile { get; set; }
    };

    public abstract class MessageBase : IMessage
    {
        public MessageBase( EMessageType aType, string aUser, IRepository aRepo, JObject aJson )
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

        public EMessageType Type { get; protected set; } = EMessageType.Unknown;

        public FileInfo SourceFile { get; set; }

        protected readonly IRepository mRepo;
    }

    public sealed class GenericMessage : MessageBase
    {
        public GenericMessage( EMessageType aType, string aUser ) : base( aType, aUser, aRepo: null, aJson: null )
        {
            Summary = Type.ToString();
        }
    }
}
